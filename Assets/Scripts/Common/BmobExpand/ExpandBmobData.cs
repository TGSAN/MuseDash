using cn.bmob.api;
using cn.bmob.io;
using cn.bmob.json;
using FormulaBase;
using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandBmobGameObject : BmobTable
{
    public String fileName { get; set; }
    public String data { get; set; }
    public BmobPointer<BmobUser> user { get; set; }

    public override void readFields(BmobInput input)
    {
        base.readFields(input);

        this.fileName = input.getString("fileName");
        this.data = input.getString("data");
        this.user = input.Get<BmobPointer<BmobUser>>("user");
    }

    public override void write(BmobOutput output, Boolean all)
    {
        base.write(output, all);

        output.Put("fileName", this.fileName);
        output.Put("data", this.data);
        output.Put("user", this.user);
    }
}

public class ExpandBmobData : MonoBehaviour
{
    private BmobUnity Bmob;

    private static ExpandBmobData instance = null;

    private const string USER_DATA_TABLE_NAME = "UserData";
    private const string USER_TABLE_NAME = "_User";

    public static ExpandBmobData Instance
    {
        get
        {
            return instance;
        }
    }

    // Use this for initialization
    private void Start()
    {
        instance = this;
        this.Bmob = this.gameObject.GetComponent<BmobUnity>();
    }

    public void FineOne(string objectId, string fileName, HttpAddDelegate rsp)
    {
        BmobQuery query = new BmobQuery();
        query.WhereEqualTo("objectId", objectId);
        Bmob.Find<ExpandBmobGameObject>(USER_DATA_TABLE_NAME, query, (resp, exception) =>
        {
            if (exception != null)
            {
                print("FineOne 查询失败, 失败原因为： " + exception.Message);
                return;
            }

            List<ExpandBmobGameObject> list = resp.results;
            foreach (ExpandBmobGameObject ebo in list)
            {
                print("FineOne 获取的对象为： " + ebo.ToString());
                FormulaHost _tempHost = FomulaHostManager.Instance.CreateHost(fileName);
                _tempHost.objectID = ebo.objectId;
                _tempHost.createAt = ebo.createdAt;
                _tempHost.updateAt = ebo.updatedAt;
                SimpleJson.JsonObject _objData = JsonAdapter.JSON.ToObject(ebo.data) as SimpleJson.JsonObject;
                _tempHost.JsonToSign(_objData);
                if (rsp != null)
                {
                    rsp(_tempHost);
                }
                CommonPanel.GetInstance().ShowWaittingPanel(false);
                // need only first one.
                return;
            }
        });
    }

    /// <summary>
    /// Add new row or Creates new table.
    ///
    /// Table name is uid.
    /// </summary>
    /// <param name="tableName">Uid.</param>
    public void Add(FormulaHost host, HttpResponseDelegate rsp)
    {
        ExpandBmobGameObject data = new ExpandBmobGameObject();
        var user = new BmobPointer<BmobUser>(ExpandBmobUser.curUser);
        data.user = user;
        data.fileName = host.GetFileName();
        data.data = host.SignToJson().ToJson();
        ExpandBmobHeartBeat.Instance.AddWait();
        Bmob.Create(USER_DATA_TABLE_NAME, data, (resp, exception) =>
        {
            ExpandBmobHeartBeat.Instance.DelWait();
            if (exception != null)
            {
                string failMsg = "添加数据失败, 失败原因为： " + exception.Message;
                print(failMsg);
                CommonPanel.GetInstance().ShowText(failMsg);
                if (rsp != null)
                {
                    rsp(false);
                }
                return;
            }

            if (host != null)
            {
                host.objectID = resp.objectId;
                host.createAt = resp.createdAt;
                host.updateAt = resp.createdAt;
                FormulaBase.FomulaHostManager.Instance.AddHost(host);

                print("创建成功, @" + host.objectID + " / " + host.GetFileName() + "/" + host.createAt);
            }
            else
            {
                print("创建失败, @" + resp.createdAt);
            }

            if (host.IsDirty)
            {
                host.IsDirty = false;
                this.UpdateRow(host, rsp);
            }
            else
            {
                CommonPanel.GetInstance().ShowWaittingPanel(false);
                if (rsp != null)
                {
                    rsp(true);
                }
            }
        });
    }

    public void UpdateList(List<FormulaHost> hosts, HttpEndResponseDelegate rsp = null)
    {
        if (hosts == null || hosts.Count <= 0)
        {
            return;
        }

        List<FormulaHost> createList = new List<FormulaHost>();
        List<FormulaHost> updateList = new List<FormulaHost>();
        List<FormulaHost> deleteList = new List<FormulaHost>();
        var dicEbInsert = new Dictionary<ExpandBmobGameObject, string>();
        var dicEbUpdate = new Dictionary<ExpandBmobGameObject, string>();
        var dicEbDelete = new Dictionary<ExpandBmobGameObject, string>();
        var user = new BmobPointer<BmobUser>(ExpandBmobUser.curUser);
        foreach (FormulaHost host in hosts)
        {
            ExpandBmobGameObject data = new ExpandBmobGameObject();
            data.fileName = host.GetFileName();
            data.data = host.SignToJson().ToJson();
            data.user = user;
            var tableName = USER_DATA_TABLE_NAME;
            if (host.objectID != null)
            {
                updateList.Add(host);
                data.objectId = host.objectID;
                dicEbUpdate.Add(data, tableName);
            }
            else
            {
                createList.Add(host);
                dicEbInsert.Add(data, tableName);
            }

            if (host.objectID != null && host.IsDelete)
            {
                deleteList.Add(host);
                dicEbDelete.Add(data, tableName);
            }
        }

        Debug.Log("--Save list with " + dicEbUpdate.Count + "/" + dicEbInsert.Count);
        BmobBatch uBatch = new BmobBatch();
        foreach (var pair in dicEbInsert)
        {
            uBatch.Create(pair.Value, pair.Key);
        }

        foreach (var pair in dicEbUpdate)
        {
            ExpandBmobGameObject _dt = pair.Key;
            uBatch.Update(pair.Value, _dt.objectId, _dt);
        }

        foreach (var pair in dicEbDelete)
        {
            ExpandBmobGameObject _dt = pair.Key;
            uBatch.Delete(pair.Value, _dt.objectId);
        }

        this.Bmob.Batch(uBatch, (resp, exception) =>
        {
            if (resp == null)
            {
                print("batch保存失败 " + exception);
                return;
            }

            ExpandBmobHeartBeat.Instance.DelWait();
            print("batch保存完毕 ");

            for (int i = 0; i < resp.Count; i++)
            {
                SimpleJson.JsonObject data = (SimpleJson.JsonObject)resp[i]["success"];

                string _oid = null;
                string _createdAt = null;
                string _updatedAt = null;
                if (data.ContainsKey("createdAt"))
                {
                    _createdAt = data["createdAt"].ToString();
                }

                if (data.ContainsKey("updatedAt"))
                {
                    _updatedAt = data["updatedAt"].ToString();
                }

                if (data.ContainsKey("objectId"))
                {
                    _oid = data["objectId"].ToString();
                }

                Debug.Log(_oid + " : cr " + _createdAt + " / ud " + _updatedAt);

                if (_oid != null && _oid.Length > 2)
                {
                    // new one
                    if (createList.Count <= 0)
                    {
                        continue;
                    }

                    FormulaHost _h = createList[0];
                    if (_createdAt != null)
                    {
                        _h.objectID = _oid;
                        _h.createAt = _createdAt;
                        _h.updateAt = _createdAt;
                    }

                    FomulaHostManager.Instance.AddHost(_h);
                    createList.RemoveAt(0);
                }
                else
                {
                    // update one
                    if (updateList.Count <= 0)
                    {
                        continue;
                    }

                    FormulaHost _h = updateList[0];
                    if (_updatedAt != null)
                    {
                        _h.updateAt = _updatedAt;
                    }

                    FomulaHostManager.Instance.AddHost(_h);
                    updateList.RemoveAt(0);
                }
            }

            for (int i = 0; i < deleteList.Count; i++)
            {
                FormulaHost _h = deleteList[i];
                FomulaHostManager.Instance.RemoveHostFromPool(_h);
            }

            if (rsp != null)
            {
                rsp(null);
            }

            CommonPanel.GetInstance().ShowWaittingPanel(false);
        });
    }

    public void DeleteList(List<FormulaHost> hosts, HttpEndResponseDelegate rsp = null)
    {
        if (hosts == null || hosts.Count <= 0)
        {
            return;
        }

        SimpleJson.JsonObject param = new SimpleJson.JsonObject();
        SimpleJson.JsonArray arrayDelete = new SimpleJson.JsonArray();
        foreach (FormulaHost host in hosts)
        {
            SimpleJson.JsonObject obj = new SimpleJson.JsonObject();
            if (host != null && host.objectID != null)
            {
                obj["objectID"] = host.objectID;
                arrayDelete.Add(obj);
            }
        }

        param["deletearray"] = arrayDelete;

        HttpEndResponseDelegate _rsp = new HttpEndResponseDelegate((cn.bmob.response.EndPointCallbackData<Hashtable> response) =>
        {
            print("删除成功 " + response);
            for (int i = 0; i < hosts.Count; i++)
            {
                FormulaHost host = hosts[i];
                if (host == null)
                {
                    continue;
                }

                FomulaHostManager.Instance.RemoveHostFromPool(host);
            }
        });

        if (rsp != null)
        {
            _rsp += rsp;
        }

        ExpandBmobCall.Instance.Call("DeleteList", param, _rsp);
    }

    public void UpdateRow(FormulaHost host, HttpResponseDelegate rsp)
    {
        var tableName = USER_DATA_TABLE_NAME;
        if (host.objectID == null && host.localObjectId == null)
        {
            host.SetLocalObjectId();
            this.Add(host, rsp);
            return;
        }

        if (host.objectID == null)
        {
            host.IsDirty = true;
            return;
        }

        ExpandBmobGameObject data = new ExpandBmobGameObject();
        data.data = host.SignToJson().ToJson();
        var user = new BmobPointer<BmobUser>(ExpandBmobUser.curUser);
        data.user = user;
        Bmob.Update(tableName, host.objectID, data, (resp, exception) =>
        {
            ExpandBmobHeartBeat.Instance.DelWait();
            if (exception != null)
            {
                string failMsg = "保存数据失败, 失败原因为： " + exception.Message;
                print(failMsg);
                CommonPanel.GetInstance().ShowText(failMsg);
                if (rsp != null)
                {
                    rsp(false);
                }
            }
            else
            {
                String updateAt = resp.updatedAt;
                if (host != null)
                {
                    host.updateAt = updateAt;
                    FormulaBase.FomulaHostManager.Instance.AddHost(host);
                    print("保存成功, @" + updateAt + " / " + host.objectID + " / " + host.GetFileName());
                }
                else
                {
                    print("保存失败, @" + updateAt);
                }

                //CommonPanel.GetInstance().ShowWaittingPanel(false);

                if (rsp != null)
                {
                    rsp(true);
                }
            }
        });
    }

    public void UpdateRow(string tableName, string objectId, IBmobWritable data, HttpResponseDelegate rsp)
    {
        ExpandBmobHeartBeat.Instance.AddWait();
        Bmob.Update(tableName, objectId, data, (resp, exception) =>
        {
            ExpandBmobHeartBeat.Instance.DelWait();
            if (exception != null)
            {
                string failMsg = "更新数据失败, 失败原因为： " + exception.Message;
                print(failMsg);
                CommonPanel.GetInstance().ShowText(failMsg);
                if (rsp != null)
                {
                    rsp(false);
                }
            }
            else
            {
                print("更新成功, @" + resp.updatedAt);
                if (rsp != null)
                {
                    rsp(true);
                }
            }
        });
    }

    public void DeleteUser(string uid, HttpResponseDelegate rsp)
    {
        ExpandBmobHeartBeat.Instance.AddWait();
        Bmob.Delete(USER_TABLE_NAME, uid, (resp, exception) =>
        {
            ExpandBmobHeartBeat.Instance.DelWait();
            if (exception != null)
            {
                string failMsg = "删除数据失败, 失败原因为： " + exception.Message;
                print(failMsg);
                CommonPanel.GetInstance().ShowText(failMsg);
                if (rsp != null)
                {
                    rsp(false);
                }

                return;
            }

            print("删除成功, @" + resp.msg);

            if (rsp != null)
            {
                rsp(true);
            }
        });
    }

    public void Delete(FormulaHost host, HttpResponseDelegate rsp)
    {
        if (host.objectID == null)
        {
            return;
        }

        ExpandBmobHeartBeat.Instance.AddWait();
        Bmob.Delete(USER_DATA_TABLE_NAME, host.objectID, (resp, exception) =>
        {
            ExpandBmobHeartBeat.Instance.DelWait();
            if (exception != null)
            {
                string failMsg = "删除数据失败, 失败原因为： " + exception.Message;
                print(failMsg);
                CommonPanel.GetInstance().ShowText(failMsg);
                if (rsp != null)
                {
                    rsp(false);
                }

                return;
            }

            print("删除成功, @" + resp.msg);
            if (host != null)
            {
                FormulaBase.FomulaHostManager.Instance.RemoveHostFromPool(host);
            }

            if (rsp != null)
            {
                rsp(true);
            }
        });
    }

    public void LoadAll(string uid, HttpEndResponseDelegate delgat)
    {
        SimpleJson.JsonObject _param = new SimpleJson.JsonObject();
        _param["objectId"] = uid;
        ExpandBmobHeartBeat.Instance.AddWait();
        Bmob.Endpoint<Hashtable>("LoadTable", _param, (resp, exception) =>
        {
            ExpandBmobHeartBeat.Instance.DelWait();
            if (exception != null)
            {
                string failMsg = "查询失败, 失败原因为： " + exception.Message;
                print(failMsg);
                CommonPanel.GetInstance().ShowText(failMsg);
                return;
            }
            if (delgat != null)
            {
                delgat(resp);
            }
        });
    }
}