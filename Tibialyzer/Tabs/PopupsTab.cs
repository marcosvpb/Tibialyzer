﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tibialyzer {
    public partial class PopupsTab : Form, TabInterface {
        ToolTip tooltip = UIManager.CreateTooltip();
        public PopupsTab() {
            InitializeComponent();
            InitializeSettings();
            ApplyLocalization();
        }

        public void InitializeSettings() {
            this.popupAnchorDropDownList.SelectedIndex = Math.Min(Math.Max(SettingsManager.getSettingInt("SimpleNotificationAnchor"), 0), 3);
            this.popupXOffsetBox.Text = SettingsManager.getSettingInt("SimpleNotificationXOffset").ToString();
            this.popupYOffsetBox.Text = SettingsManager.getSettingInt("SimpleNotificationYOffset").ToString();
            this.notificationWidthBox.Text = SettingsManager.getSettingInt("SimpleNotificationWidth").ToString();
            this.showCopyButtonCheckbox.Checked = SettingsManager.getSettingBool("SimpleNotificationCopyButton");
            this.popupTypeDropDownList.SelectedIndex = SettingsManager.getSettingBool("UseRichNotificationType") ? 1 : 0;

            popupSpecificItemBox.Items.Clear();
            foreach (string str in SettingsManager.getSetting("NotificationItems")) {
                popupSpecificItemBox.Items.Add(str);
            }
            popupSpecificItemBox.ItemsChanged += PopupSpecificItemBox_ItemsChanged;
            popupSpecificItemBox.verifyItem = StorageManager.itemExists;
            popupSpecificItemBox.RefreshControl();

            popupConditionBox.Items.Clear();
            foreach (string str in SettingsManager.getSetting("NotificationConditions")) {
                popupConditionBox.Items.Add(str);
            }
            popupConditionBox.ItemsChanged += PopupConditionBox_ItemsChanged;
            popupConditionBox.verifyItem = NotificationConditionManager.ValidCondition;
            popupConditionBox.RefreshControl();

            popupDurationSlider.Value = Math.Min(Math.Max(SettingsManager.getSettingInt("PopupDuration"), popupDurationSlider.Minimum), popupDurationSlider.Maximum);
            popupDurationHeader.Text = String.Format("Popup Duration ({0}s)", popupDurationSlider.Value);
        }

        public void ApplyLocalization() {
            tooltip.RemoveAll();
            sizeWidthHeader.Text = Tibialyzer.Translation.PopupsTab.sizeWidthHeader;
            popupTestButton.Text = Tibialyzer.Translation.PopupsTab.popupTestButton;
            positionOffsetHeader.Text = Tibialyzer.Translation.PopupsTab.positionOffsetHeader;
            popupItemsHeader.Text = Tibialyzer.Translation.PopupsTab.popupItemsHeader;
            goldCapRatioPopupHeader.Text = Tibialyzer.Translation.PopupsTab.goldCapRatioPopupHeader;
            popupConditionsHeader.Text = Tibialyzer.Translation.PopupsTab.popupConditionsHeader;
            showCopyButtonCheckbox.Text = Tibialyzer.Translation.PopupsTab.showCopyButtonCheckbox;
            popupDurationHeader.Text = Tibialyzer.Translation.PopupsTab.popupDurationHeader;
            setGoldCapRatioButton.Text = Tibialyzer.Translation.PopupsTab.setGoldCapRatioButton;
            itemValuePopupHeader.Text = Tibialyzer.Translation.PopupsTab.itemValuePopupHeader;
            anchorHeader.Text = Tibialyzer.Translation.PopupsTab.anchorHeader;
            popupTypeHeader.Text = Tibialyzer.Translation.PopupsTab.popupTypeHeader;
            testPopupMessageHeader.Text = Tibialyzer.Translation.PopupsTab.testPopupMessageHeader;
            testDisplayButton.Text = Tibialyzer.Translation.PopupsTab.testDisplayButton;
            clearDisplayButton.Text = Tibialyzer.Translation.PopupsTab.clearDisplayButton;
            testingHeader.Text = Tibialyzer.Translation.PopupsTab.testingHeader;
            setItemValuePopupButton.Text = Tibialyzer.Translation.PopupsTab.setItemValuePopupButton;
            tooltip.SetToolTip(popupTypeDropDownList, Tibialyzer.Translation.PopupsTab.popupTypeDropDownListTooltip);
            tooltip.SetToolTip(popupTestButton, Tibialyzer.Translation.PopupsTab.popupTestButtonTooltip);
            tooltip.SetToolTip(setGoldCapRatioButton, Tibialyzer.Translation.PopupsTab.setGoldCapRatioButtonTooltip);
            tooltip.SetToolTip(popupAnchorDropDownList, Tibialyzer.Translation.PopupsTab.popupAnchorDropDownListTooltip);
            tooltip.SetToolTip(setItemValuePopupButton, Tibialyzer.Translation.PopupsTab.setItemValuePopupButtonTooltip);
            int popupTypeDropDownListSelectedIndex = popupTypeDropDownList.SelectedIndex;
            popupTypeDropDownList.Items.Clear();
            popupTypeDropDownList.Items.Add(Tibialyzer.Translation.PopupsTab.popupTypeDropDownList_0);
            popupTypeDropDownList.Items.Add(Tibialyzer.Translation.PopupsTab.popupTypeDropDownList_1);
            popupTypeDropDownList.SelectedIndex = popupTypeDropDownListSelectedIndex;
            int popupAnchorDropDownListSelectedIndex = popupAnchorDropDownList.SelectedIndex;
            popupAnchorDropDownList.Items.Clear();
            popupAnchorDropDownList.Items.Add(Tibialyzer.Translation.PopupsTab.popupAnchorDropDownList_0);
            popupAnchorDropDownList.Items.Add(Tibialyzer.Translation.PopupsTab.popupAnchorDropDownList_1);
            popupAnchorDropDownList.Items.Add(Tibialyzer.Translation.PopupsTab.popupAnchorDropDownList_2);
            popupAnchorDropDownList.Items.Add(Tibialyzer.Translation.PopupsTab.popupAnchorDropDownList_3);
            popupAnchorDropDownList.SelectedIndex = popupAnchorDropDownListSelectedIndex;
        }

        private void PopupConditionBox_ItemsChanged(object sender, EventArgs e) {
            if (MainForm.prevent_settings_update) return;
            List<string> conditions = new List<string>();
            foreach (object obj in popupConditionBox.Items) {
                conditions.Add(obj.ToString());
            }
            SettingsManager.setSetting("NotificationConditions", conditions);
        }

        private void PopupSpecificItemBox_ItemsChanged(object sender, EventArgs e) {
            if (MainForm.prevent_settings_update) return;
            List<string> items = new List<string>();

            foreach (object obj in (sender as PrettyListBox).Items) {
                items.Add(obj.ToString());
            }
            SettingsManager.setSetting("NotificationItems", items);
        }

        private void setValuePopupButton_Click(object sender, EventArgs e) {
            int value = 0;
            if (int.TryParse(popupValueBox.Text.Trim(), out value)) {
                string valueString = String.Format("item.value >= {0}", value);
                for (int i = 0; i < popupConditionBox.Items.Count; i++) {
                    string testObject = popupConditionBox.Items[i].ToString().Replace(" ", "");
                    if (testObject.Trim().Length == 0 || testObject.StartsWith("item.value>=")) {
                        popupConditionBox.Items[i] = valueString;
                        if (testObject.Trim().Length == 0) {
                            popupConditionBox.Items.Add("");
                        }
                        PopupConditionBox_ItemsChanged(popupConditionBox, null);
                        return;
                    }
                }
                popupConditionBox.Items.Add(valueString);
                PopupConditionBox_ItemsChanged(popupConditionBox, null);
            }
        }

        private void popupSetGoldCapRatioButton_Click(object sender, EventArgs e) {
            int value = 0;
            if (int.TryParse(popupGoldCapRatioBox.Text.Trim(), out value)) {
                string valueString = String.Format("(item.value / item.capacity) >= {0}", value);
                for (int i = 0; i < popupConditionBox.Items.Count; i++) {
                    string testObject = popupConditionBox.Items[i].ToString().Replace(" ", "");
                    if (testObject.Trim().Length == 0 || testObject.StartsWith("(item.value/item.capacity)>=")) {
                        popupConditionBox.Items[i] = valueString;
                        if (testObject.Trim().Length == 0) {
                            popupConditionBox.Items.Add("");
                        }
                        PopupConditionBox_ItemsChanged(popupConditionBox, null);
                        return;
                    }
                }
                popupConditionBox.Items.Add(valueString);
                PopupConditionBox_ItemsChanged(popupConditionBox, null);
            }
        }

        private void simpleAnchor_SelectedIndexChanged(object sender, EventArgs e) {
            if (MainForm.prevent_settings_update) return;

            SettingsManager.setSetting("SimpleNotificationAnchor", (sender as ComboBox).SelectedIndex);
        }

        private void simpleXOffset_TextChanged(object sender, EventArgs e) {
            if (MainForm.prevent_settings_update) return;

            int xOffset;
            if (int.TryParse((sender as TextBox).Text, out xOffset)) {
                SettingsManager.setSetting("SimpleNotificationXOffset", xOffset);
            }
        }

        private void simpleYOffset_TextChanged(object sender, EventArgs e) {
            if (MainForm.prevent_settings_update) return;

            int yOffset;
            if (int.TryParse((sender as TextBox).Text, out yOffset)) {
                SettingsManager.setSetting("SimpleNotificationYOffset", yOffset);
            }
        }

        private void simpleTestDisplay_Click(object sender, EventArgs e) {
            CommandManager.ExecuteCommand("exp@");
        }

        private void clearNotifications_Click(object sender, EventArgs e) {
            CommandManager.ExecuteCommand("close@");
        }

        private void popupTestButton_Click(object sender, EventArgs e) {
            string message = popupTestLootBox.Text;
            if (message[5] == ':') { //if the time stamp is in the form of hh:mm: (i.e. flash client format) remove the second colon
                message = message.Remove(5, 1);
            }
            var parseResult = Parser.ParseLootMessage(message);
            if (parseResult != null) {
                bool showNotification = PopupManager.ShowDropNotification(new Tuple<Creature, List<Tuple<Item, int>>, string>(parseResult.Item1, parseResult.Item2, message));
                if (showNotification) {
                    MainForm.mainForm.Invoke((MethodInvoker)delegate {
                        PopupManager.ShowSimpleNotification(new SimpleLootNotification(parseResult.Item1, parseResult.Item2, message));
                    });
                } else {
                    MainForm.mainForm.Invoke((MethodInvoker)delegate {
                        PopupManager.ShowSimpleNotification(new SimpleTextNotification(null, "Conditions Not Matched", "This loot message did not generate a popup."));
                    });
                }
            } else {
                MainForm.mainForm.DisplayWarning(String.Format("Could not parse loot message: {0}", popupTestLootBox.Text));
            }
        }

        private void popupTestLootBox_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == '\r') {
                popupTestButton_Click(popupTestButton, null);
                e.Handled = true;
            }
        }
        
        private void popupDurationSlider_Scroll(object sender, EventArgs e) {
            if (MainForm.prevent_settings_update) return;

            SettingsManager.setSetting("PopupDuration", popupDurationSlider.Value);
            popupDurationHeader.Text = String.Format("Popup Duration ({0}s)", popupDurationSlider.Value);
        }

        private void notificationWidthBox_TextChanged(object sender, EventArgs e) {
            if (MainForm.prevent_settings_update) return;

            int width;
            if (int.TryParse((sender as TextBox).Text, out width)) {
                SettingsManager.setSetting("SimpleNotificationWidth", width);
            }
        }

        private void showCopyButtonCheckbox_CheckedChanged(object sender, EventArgs e) {
            if (MainForm.prevent_settings_update) return;

            SettingsManager.setSetting("SimpleNotificationCopyButton", showCopyButtonCheckbox.Checked);
        }
        private void notificationTypeBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (MainForm.prevent_settings_update) return;

            SettingsManager.setSetting("UseRichNotificationType", ((sender as ComboBox).SelectedIndex == 1).ToString());
        }
    }
}
