
using System;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using System.Text.RegularExpressions;

// Json config file parsed into this object
public class Config
{
    public List<string> paths {get; set;}
}
