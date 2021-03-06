﻿namespace Minary.Domain.MacVendor
{
  using Minary.DataTypes.Enum;
  using Minary.LogConsole.Main;
  using System;
  using System.Collections;
  using System.IO;
  using System.Text.RegularExpressions;


  public class MacVendorHandler
  {

    #region MEMBERS
    
    private string macVendorList = @"data\MACVendors.txt";
    private Hashtable macVendorMap;

    #endregion


    #region PUBLIC

    /// <summary>
    /// Initializes a new instance of the <see cref="MacVendorHandler"/> class.
    ///
    /// </summary>
    public MacVendorHandler()
    {
      // Load MacAddress vendor list
      this.macVendorMap = new Hashtable();
      this.LoadMacVendorList();
    }


    /// <summary>
    ///
    /// </summary>
    /// <param name="macAddress"></param>
    /// <returns></returns>
    public string GetVendorByMac(string macAddress)
    {
      var retVal = string.Empty;
      var vendor = string.Empty;
      Match match;

      if (string.IsNullOrEmpty(macAddress))
      {
        return retVal;
      }

      // Determine vendor
      vendor = string.Empty;
      if ((match = Regex.Match(macAddress, @"([\da-f]{1,2})[:\-]{1}([\da-f]{1,2})[:\-]{1}([\da-f]{1,2})[:\-]{1}.*", RegexOptions.IgnoreCase)).Success)
      {
        var octet1 = match.Groups[1].Value.ToString();
        var octet2 = match.Groups[2].Value.ToString();
        var octet3 = match.Groups[3].Value.ToString();
        var vendorId = $"{octet1}{octet2}{octet3}".ToLower();

        retVal = this.macVendorMap.ContainsKey(vendorId) ? this.macVendorMap[vendorId].ToString() : string.Empty;
      }

      return retVal;
    }

    #endregion


    #region PRIVATE

    /// <summary>
    ///
    /// </summary>
    private void LoadMacVendorList()
    {
      var tmpLine = string.Empty;
      StreamReader reader = null;
      var delimiterCharacters = "\t ".ToCharArray();
      var macAddress = string.Empty;
      var vendorName = string.Empty;

      try
      {
        reader = new StreamReader(this.macVendorList);
        while ((tmpLine = reader.ReadLine()) != null)
        {
          tmpLine = tmpLine.Trim();

          try
          {
            string[] splitter = tmpLine.Split(delimiterCharacters, 2);

            if (splitter.Length == 2)
            {
              macAddress = splitter[0].ToLower();
              vendorName = splitter[1];
              this.macVendorMap.Add(macAddress, vendorName);
            }
          }
          catch (Exception)
          {
            LogCons.Inst.Write(LogLevel.Error, "Unable to load MacAddress/Vendor pair: {0}/{1}   ({2})", tmpLine, macAddress, vendorName);
          }
        }
      }
      catch (FileNotFoundException)
      {
        LogCons.Inst.Write(LogLevel.Error, "{0} not found!", this.macVendorList);
      }
      catch (Exception ex)
      {
        LogCons.Inst.Write(LogLevel.Error, "Error occurred while opening {0}: {1}", this.macVendorList, ex.StackTrace);
      }
      finally
      {
        if (reader != null)
        {
          reader.Close();
        }
      }
    }

    #endregion

  }
}
