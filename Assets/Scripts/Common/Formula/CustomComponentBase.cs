using System;
using System.Collections.Generic;
using System.Linq;

namespace FormulaBase
{
    public enum FileType
    {
        Account,
        Role,
        Equip,
        Material,
        Pet,
        Task,
    }

    public class CustomComponentBase
    {
        private FormulaHost currentHost;
        private Dictionary<string, FormulaHost> list;

        public CustomComponentBase()
        {
        }

        /// <summary>
        /// Gets or sets the host.
        ///
        /// 当前活动host
        /// </summary>
        /// <value>The host.</value>
        public FormulaHost Host
        {
            get
            {
                return this.GetCurrentHost();
            }

            set
            {
                this.SetCurrentHost(value);
            }
        }

        /// <summary>
        /// Gets the host list.
        ///
        /// 本模块类型下的host集
        /// 如果为空，可尝试用
        /// this.GetList (HOST_IDX);
        /// 初始化
        /// </summary>
        /// <value>The host list.</value>
        public Dictionary<string, FormulaHost> HostList
        {
            get
            {
                return this.list;
            }
        }

        private FormulaHost GetCurrentHost()
        {
            return this.currentHost;
        }

        private void SetCurrentHost(FormulaHost host)
        {
            this.currentHost = host;
        }

        /// <summary>
        /// Gets the list.
        ///
        /// 获得某个类型的全部host
        /// </summary>
        /// <returns>The list.</returns>
        /// <param name="hostType">Host type.</param>
        public Dictionary<string, FormulaHost> GetList(int hostType)
        {
            if (this.list != null)
            {
                return this.list;
            }

            string fileName = FomulaHostManager.Instance.GetFileNameByHostType(hostType);
            if (fileName == null)
            {
                return null;
            }

            this.list = FomulaHostManager.Instance.GetHostListByFileName(fileName);
            return this.list;
        }

        /// <summary>
        /// Gets the list.
        ///
        /// 获得某个类型的全部host
        /// </summary>
        /// <returns>The list.</returns>
        /// <param name="fileName">File name.</param>
        public Dictionary<string, FormulaHost> GetList(string fileName)
        {
            if (this.list != null)
            {
                return this.list;
            }
            this.list = FomulaHostManager.Instance.GetHostListByFileName(fileName);
            return this.list;
        }

        /// <summary>
        /// Gets the identifier.
        ///
        /// 获得SignKeys.ID的值
        /// </summary>
        /// <returns>The identifier.</returns>
        /// <param name="defaultValue">Default value.</param>
        public int GetId(int defaultValue = 0)
        {
            if (this.currentHost == null)
            {
                return defaultValue;
            }

            return this.currentHost.GetDynamicIntByKey(SignKeys.ID, defaultValue);
        }

        /// <summary>
        /// Gets the host by key value.
        ///
        /// 通过键值对查找单个host
        /// </summary>
        /// <returns>The host by key value.</returns>
        /// <param name="signKey">Sign key.</param>
        /// <param name="value">Value.</param>
        public FormulaHost GetHostByKeyValue(string signKey, int value)
        {
            if (this.list == null)
            {
                return null;
            }
            foreach (string oid in this.list.Keys)
            {
                FormulaHost _h = this.list[oid];
                if (_h == null)
                {
                    continue;
                }

                if (_h.GetDynamicIntByKey(signKey) == value)
                {
                    return _h;
                }
            }

            return null;
        }

        public FormulaHost GetHostByKeyValue(string signKey, string value)
        {
            if (this.list == null)
            {
                return null;
            }

            foreach (string oid in this.list.Keys)
            {
                FormulaHost _h = this.list[oid];
                if (_h == null)
                {
                    continue;
                }

                if (_h.GetDynamicStrByKey(signKey) == value)
                {
                    return _h;
                }
            }

            return null;
        }

        /// <summary>
        /// Saves the modified hosts.
        ///
        /// 批量save数据有变动的host
        /// </summary>
        public void SaveModifiedHosts(HttpEndResponseDelegate rsp = null)
        {
            if (this.HostList == null)
            {
                return;
            }

            List<FormulaHost> _list = new List<FormulaHost>();
            foreach (string oid in this.HostList.Keys)
            {
                FormulaHost _h = this.HostList[oid];
                if (_h == null)
                {
                    continue;
                }

                if (_h.GetDynamicObjByKey(FormulaBase.SING_KEY_DATA_MODIFIED) == null)
                {
                    continue;
                }

                _list.Add(_h);
            }

            FormulaHost.SaveList(_list, rsp);
        }

        public static void DeleteAllHost(HttpEndResponseDelegate callFunc = null)
        {
            var enums = Enum.GetNames(typeof(FileType));
            var list = new List<FormulaHost>();
            foreach (var e in enums)
            {
                var hostList = FomulaHostManager.Instance.GetHostListByFileName(e.ToString());
                list.AddRange(hostList.Values);
            }
            FormulaHost.DeleteList(list, callFunc);
        }
    }
}