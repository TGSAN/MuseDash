using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FormulaBase;

public class UIPhaseOnClickBase {
	private static void HideSelf(UIPhaseHelper uph, GameObject gameObject) {
		if (!uph.isClickHideSelf) {
			return;
		}

		if (uph.gameObject == null) {
			return;
		}

		if (!uph.gameObject.activeSelf) {
			return;
		}

		UIPhaseBase upb = uph.gameObject.GetComponent<UIPhaseBase> ();
		if (upb == null) {
			uph.gameObject.SetActive (false);
			return;
		}

		upb.Hide ();
	}

	private static void HideRoot(UIPhaseHelper uph, GameObject gameObject) {
		if (!uph.isClickHideParent) {
			return;
		}

		if (uph.root == null) {
			return;
		}

		if (!uph.root.activeSelf) {
			return;
		}

		UIPhaseBase upb = uph.root.GetComponent<UIPhaseBase> ();
		if (upb == null) {
			uph.root.SetActive (false);
			return;
		}

		upb.Hide ();
	}

	private static void ShowResponse(UIPhaseHelper uph, GameObject gameObject) {
		if (uph.clickResponseObjects != null || uph.clickResponseObjects.Count > 0) {
			int _idx = -1;
			foreach (GameObject _obj in uph.clickResponseObjects) {
				_idx += 1;
				bool _isShow = true;
				string _showAnimation = string.Empty;
				if (_idx < uph.clickResponseObjectsIsShow.Count) {
					_isShow = uph.clickResponseObjectsIsShow [_idx];
				}

				if (_idx < uph.clickResponseObjectAnimations.Count) {
					_showAnimation = uph.clickResponseObjectAnimations [_idx];
				}

				if (_obj == null) {
					continue;
				}

				GameObject instanceObject = _obj;
				UIPhaseBase upb = instanceObject.GetComponent<UIPhaseBase> ();
				if (upb == null) {
					instanceObject.SetActive (_isShow);
					continue;
				}

				if (_isShow) {
					upb.Show (_showAnimation);
				} else {
					upb.Hide (_showAnimation);
				}
			}
		}

		if (uph.root == null || UISceneHelper.Instance == null) {
			return;
		}

		if (uph.clickResponseParentNames != null || uph.clickResponseParentNames.Count > 0) {
			int _idx = -1;
			foreach (string _name in uph.clickResponseParentNames) {
				_idx += 1;
				if (_name == null || _name == string.Empty) {
					continue;
				}

				bool _isShow = true;
				string _showAnimation = string.Empty;
				if (_idx < uph.clickResponseParentNamesIsShow.Count) {
					_isShow = uph.clickResponseParentNamesIsShow [_idx];
				}

				if (_idx < uph.clickResponseParentAnimations.Count) {
					_showAnimation = uph.clickResponseParentAnimations [_idx];
				}

				// root object find under the same parent.
				GameObject _trs = UISceneHelper.Instance.FindDymWidget (_name);
				if (_trs == null) {
					Debug.Log (_name + " not in scene.");
					continue;
				}

				GameObject instanceObject = _trs;
				UIRootHelper _urh = instanceObject.GetComponent<UIRootHelper> ();
				if (_urh != null) {
					GameObject t = UISceneHelper.Instance.FindDymWidget (instanceObject.name);
					if (t == null) {
						Debug.Log (instanceObject.name + " not in scene.");
						continue;
					}

					instanceObject = t;
				}

				if (instanceObject == null) {
					Debug.Log (instanceObject.name + " not in scene.");
					continue;
				}

				UISceneHelper ush = instanceObject.GetComponent<UISceneHelper> ();
				if (ush != null) {
					continue;
				}

				UIPhaseBase upb = instanceObject.GetComponent<UIPhaseBase> ();
				if (upb == null) {
					instanceObject.SetActive (_isShow);
					continue;
				}

				if (_isShow) {
					upb.Show (_showAnimation);
				} else {
					upb.Hide (_showAnimation);
				}
			}
		}
	}

	public static void OnDo(GameObject gameObject) {
		UIPhaseHelper uph = gameObject.GetComponent<UIPhaseHelper> ();
		if (uph == null) {
			return;
		}

		ShowResponse (uph, gameObject);
		HideSelf (uph, gameObject);
		HideRoot (uph, gameObject);
	}
}
