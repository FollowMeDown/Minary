﻿namespace Minary.Form.ArpScan.Task
{
  using Minary.Form.ArpScan.DataTypes;
  using System;
  using System.Net;


  public class ArpScan
  {

    #region PUBLIC

    /// <summary>
    /// Initializes a new instance of the <see cref="ArpScan"/> class.
    ///
    /// </summary>
    public ArpScan()
    {
    }


    /// <summary>
    ///
    /// </summary>
    public void StopArpScan()
    {
    }


    /// <summary>
    ///
    /// </summary>
    /// <param name="arpConfig"></param>
    public void StartArpScan(ArpScanConfig arpConfig)
    {
      IPAddress startIp;
      IPAddress stopIp;
      int startIpInt;
      int stopIpInt;

      // Validate input parameters.
      if (string.IsNullOrEmpty(arpConfig.InterfaceId))
      {
        throw new Exception("Something is wrong with the interface ID");
      }
      else if (!IPAddress.TryParse(arpConfig.NetworkStartIp, out startIp))
      {
        throw new Exception("Something is wrong with the start IpAddress address");
      }
      else if (!IPAddress.TryParse(arpConfig.NetworkStopIp, out stopIp))
      {
        throw new Exception("Something is wrong with the stop IpAddress address");
      }
      else if (IpHelper.Compare(startIp, stopIp) > 0)
      {
        throw new Exception("Start IpAddress address is greater than stop IpAddress address");
      }

      // If the network system range is above the defined limit change
      // the stop-address to the maximum upper limit.
      startIpInt = IpHelper.IPAddressToInt(startIp);
      stopIpInt = IpHelper.IPAddressToInt(stopIp);
      if (stopIpInt - startIpInt > arpConfig.MaxNumberSystemsToScan && arpConfig.MaxNumberSystemsToScan > 0)
      {
        int tmpStopIpInt = startIpInt + arpConfig.MaxNumberSystemsToScan;
        arpConfig.NetworkStopIp = IpHelper.IntToIpString(tmpStopIpInt);
      }
    }

    #endregion

  }
}
