﻿namespace Minary.Form
{
  using Minary.Common;
  using Minary.DataTypes.Enum;
  using Minary.LogConsole.Main;
  using System;
  using System.ComponentModel;
  using System.IO;
  using System.Linq;
  using System.Windows.Forms;


  public partial class MinaryMain
  {

    #region MEMBERS

    private BackgroundWorker bgwOnStartAttack;

    #endregion


    #region PROPERTIES

    public TabPage TabPagePluginCatalog { get { return this.tp_MinaryPluginCatalog; } }

    public TabControl TCPlugins { get { return this.tc_Plugins; } }

    public BindingList<PluginTableRecord> DgvUsedPlugins { get { return this.usedPlugins; } }

    #endregion


    #region PUBLIC

    public delegate void StartAttacksOnBackgroundDelegate();
    public void StartAttacksOnBackground()
    {
      if (this.InvokeRequired)
      {
        this.BeginInvoke(new StartAttacksOnBackgroundDelegate(this.StartAttacksOnBackground), new object[] { });
        return;
      }
      
      // Another OnStartAttack instance is running
      if (this.bgwOnStartAttack.IsBusy == true)
      {
        LogCons.Inst.Write(LogLevel.Warning, "Another instance of the OnStartAttack back ground worker is already running.");

      // Fail if selected interface is invalid
      }
      else if (this.cb_Interfaces.SelectedIndex < 0)
      {
        string message = "No network interface selected";
        MessageDialog.Inst.ShowWarning(string.Empty, message, this);

      // Notify user to select at least one target system
      }
      else if (Debugging.IsDebuggingOn == false &&
               this.arpScanHandler.TargetList.Where(elem => elem.Attack == true).Count() <= 0)
      {
        string message = "You must select at least one target system.";
        MessageDialog.Inst.ShowWarning(string.Empty, message, this);


      // Stop the attack
      }
      else if (this.attackStarted == false)
      {
        this.bgwOnStartAttack.RunWorkerAsync();


      // In any other case stop a running attack
      }
      else
      {
        this.Cursor = Cursors.WaitCursor;
        this.StopAttack();
        this.Cursor = Cursors.Default;
      }
    }

    #endregion


    #region PRIVATE

    /// <summary>
    ///
    /// </summary>
    private void DisableGuiElements()
    {
      this.bt_Attack.BackgroundImage = Properties.Resources.StopBig;
      this.bt_ScanLan.Enabled = false;
      this.cb_Interfaces.Enabled = false;
      this.dgv_MainPlugins.Enabled = false;
      this.tb_NetworkStartIp.Enabled = false;
      this.tb_NetworkStopIp.Enabled = false;
      this.tsmi_LoadTemplate.Enabled = false;
      this.tsmi_GetUpdates.Enabled = false;
      this.tsmi_Exit.Enabled = false;
      this.tsmi_ResetMinary.Enabled = false;
      this.tsmi_DetectInterfaces.Enabled = false;
      this.tsmi_Debugging.Enabled = false;
      this.tsmi_LoadTemplate.Enabled = false;
      this.tsmi_CreateTemplate.Enabled = false;
      this.tsmi_UnloadTemplate.Enabled = false;
    }


    /// <summary>
    ///
    /// </summary>
    private void EnableGuiElements()
    {
      this.bt_Attack.BackgroundImage = Properties.Resources.StartBig;
      this.bt_ScanLan.Enabled = true;
      this.cb_Interfaces.Enabled = true;
      this.dgv_MainPlugins.Enabled = true;
      this.tb_NetworkStartIp.Enabled = true;
      this.tb_NetworkStopIp.Enabled = true;
      this.tsmi_LoadTemplate.Enabled = true;
      this.tsmi_GetUpdates.Enabled = true;
      this.tsmi_Exit.Enabled = true;
      this.tsmi_ResetMinary.Enabled = true;
      this.tsmi_DetectInterfaces.Enabled = true;
      this.tsmi_Debugging.Enabled = true;
      this.tsmi_LoadTemplate.Enabled = true;
      this.tsmi_CreateTemplate.Enabled = true;
      this.tsmi_UnloadTemplate.Enabled = true;
    }


    private MinaryFileType DetermineFileType(string filePath)
    {
      if (this.templateTaskLayer.IsFileATemplate(filePath))
      {
        return MinaryFileType.TemplateFile;
      }

      return MinaryFileType.Undetermined;
    }


    private void LoadUserTemplate(string cmdLineArgument)
    {
      MinaryFileType fileType = this.DetermineFileType(cmdLineArgument);

      if (fileType != MinaryFileType.TemplateFile)
      {
        this.pluginHandler.RestoreLastPluginLoadState();
      }

      Template.Presentation.LoadTemplate loadTemplatePresentation = null;
      try
      {
        loadTemplatePresentation = new Template.Presentation.LoadTemplate(this, cmdLineArgument);
      }
      catch (Exception ex)
      {
        string message = string.Format("Error 1 occurred while loading template file \"{0}\".\r\n\r\n{1}", Path.GetFileName(cmdLineArgument), ex.Message);
        LogCons.Inst.Write(LogLevel.Error, message);
        MessageDialog.Inst.ShowWarning(string.Empty, message, this);
      }

      try
      {
        loadTemplatePresentation.ShowDialog();
      }
      catch (Exception ex)
      {
        string message = string.Format("Error 2 occurred while loading template file \"{0}\".\r\n\r\n{1}", Path.GetFileName(cmdLineArgument), ex.Message);
        LogCons.Inst.Write(LogLevel.Error, message);
        MessageDialog.Inst.ShowWarning(string.Empty, message, this);
      }

      try
      {
        if (loadTemplatePresentation != null &&
            loadTemplatePresentation.TemplateData != null &&
            !string.IsNullOrEmpty(loadTemplatePresentation.TemplateData.TemplateConfig.Name))
        {
          this.tb_TemplateName.Text = loadTemplatePresentation.TemplateData.TemplateConfig.Name;
        }
      }
      catch (Exception ex)
      {
        string message = string.Format("Error 3 occurred while loading template file \"{0}\".\r\n\r\n{1}", Path.GetFileName(cmdLineArgument), ex.Message);
        LogCons.Inst.Write(LogLevel.Error, message);
        MessageDialog.Inst.ShowWarning(string.Empty, message, this);

        this.pluginHandler.RestoreLastPluginLoadState();
      }
    }

    #endregion

  }
}
