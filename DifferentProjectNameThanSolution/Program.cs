using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MetaProgrammer;

namespace DifferentProjectNameThanSolution
{
    class Program
    {
        static void Main(string[] args)
        {
            var cc = new ClassCreator();
            cc.CreateClass("Test", "TestClass");
        }

    }
}
