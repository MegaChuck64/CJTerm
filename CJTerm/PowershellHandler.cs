using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace CJTerm;

public static class PowerhshellHandler
{
    private static PowerShell ps = PowerShell.Create();

    public static List<string> Command(string script)
    {
        var result = new List<string>();

        string errorMsg = string.Empty;

        ps.AddScript(script);
        ps.AddCommand("Out-String");

        PSDataCollection<PSObject> outputCollection = new();
        ps.Streams.Error.DataAdded += (object sender, DataAddedEventArgs e) =>
        {
            errorMsg = ((PSDataCollection<ErrorRecord>)sender)[e.Index].ToString();
        };

        IAsyncResult commandResult = ps.BeginInvoke<PSObject, PSObject>(null, outputCollection);

        ps.EndInvoke(commandResult);


        foreach (var outputItem in outputCollection)
        {

            if (outputItem.BaseObject != null)
            {
                if (outputItem.BaseObject.ToString()?.Contains("\r\n") ?? false)
                {
                    var splt = outputItem.BaseObject.ToString()?.Split("\r\n");
                    if (splt != null)
                    {
                        foreach (var s in splt)
                        {
                            result.Add(s);
                        }
                    }
                }
                else
                {
                    if (outputItem.BaseObject.ToString() is string outp)
                        result.Add(outp);
                }
            }
        }

        ps.Commands.Clear();

        if (!string.IsNullOrEmpty(errorMsg))
            result.Add(errorMsg);

        return result;
    }
}