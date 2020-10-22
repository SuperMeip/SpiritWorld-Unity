using SpiritWorld.Inventories.Items;
using System;
using UnityEditor;
using UnityEngine;

public static class EditorList {
	static GUIContent MoveDownButtonContent = new GUIContent("\u21b4", "move down");
	static GUIContent MoveUpButtonContent = new GUIContent("\u21b3", "move up");
	static GUIContent DuplicateButtonContent = new GUIContent("+", "duplicate");
	static GUIContent DeleteButtonContent = new GUIContent("-", "delete");
	static GUIContent AddButtonContent = new GUIContent("+", "add element");
	static GUILayoutOption MiniButtonWidth = GUILayout.Width(20f);

	public static void Show(SerializedProperty list, EditorListOption options = EditorListOption.Default, Func<SerializedProperty, int, string> getCustomLabel = null, string customTitle = "") {
		if (!list.isArray) {
			EditorGUILayout.HelpBox(list.name + " is neither an array nor a list!", MessageType.Error);
			return;
		}

		bool showListLabel = (options & EditorListOption.ListLabel) != 0;
		bool showListSize = (options & EditorListOption.ListSize) != 0;

		if (showListLabel) {
			if (customTitle != "") {
				EditorGUILayout.PropertyField(list, new GUIContent(customTitle), false);
			} else {
				EditorGUILayout.PropertyField(list, false);
			}
			EditorGUI.indentLevel += 1;
		}
		if (!showListLabel || list.isExpanded) {
			if (showListSize) {
				EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
			}
			ShowElements(list, options, getCustomLabel);
		}
		if (showListLabel) {
			EditorGUI.indentLevel -= 1;
		}
	}

	static void ShowElements(SerializedProperty list, EditorListOption options, Func<SerializedProperty, int, string> getCustomLabel) {
		bool showElementLabels = (options & EditorListOption.ElementLabels) != 0;
		bool showButtons = (options & EditorListOption.Buttons) != 0;
		bool buttonsAreBelow = (options & EditorListOption.ButtonsBelow) != 0;

		for (int index = 0; index < list.arraySize; index++) {
			if (showButtons && !buttonsAreBelow) {
				EditorGUILayout.BeginHorizontal();
			}

			/// the array item
			if (showElementLabels) {
				if (getCustomLabel != null) {
					SerializedProperty element = list.GetArrayElementAtIndex(index);
					EditorGUILayout.PropertyField(element, new GUIContent(getCustomLabel(element, index)));
				} else {
					EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(index));
				}
			} else {
				EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(index), GUIContent.none);
			}

			/// for showing the buttons
			if (showButtons && list.GetArrayElementAtIndex(index).isExpanded) {
				if (buttonsAreBelow) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Space(10);
				}
				ShowButtons(list, index);
				EditorGUILayout.EndHorizontal();
			}
		}

		if (showButtons && GUILayout.Button(AddButtonContent, EditorStyles.miniButton)) {
			list.arraySize += 1;
		}
	}

	private static void ShowButtons(SerializedProperty list, int index) {
		if (GUILayout.Button(MoveUpButtonContent, EditorStyles.miniButtonLeft, MiniButtonWidth)) {
			list.MoveArrayElement(index, index - 1);
		}
		if (GUILayout.Button(MoveDownButtonContent, EditorStyles.miniButtonMid, MiniButtonWidth)) {
			list.MoveArrayElement(index, index + 1);
		}
		if (GUILayout.Button(DuplicateButtonContent, EditorStyles.miniButtonMid, MiniButtonWidth)) {
			list.InsertArrayElementAtIndex(index);
		}
		if (GUILayout.Button(DeleteButtonContent, EditorStyles.miniButtonRight, MiniButtonWidth)) {
			int oldSize = list.arraySize;
			list.DeleteArrayElementAtIndex(index);
			if (list.arraySize == oldSize) {
				list.DeleteArrayElementAtIndex(index);
			}
		}
	}
}

[Flags]
public enum EditorListOption {
	None = 0,
	ListSize = 1,
	ListLabel = 2,
	ElementLabels = 4,
	Buttons = 8,
	ButtonsBelow = 16,
	Default = ListSize | ListLabel | ElementLabels,
	NoElementLabels = ListSize | ListLabel,
	All = Default | Buttons,
	AllBelow = All | ButtonsBelow
}