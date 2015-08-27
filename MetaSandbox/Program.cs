using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MetaSandbox
{
    class Program
    {

        private static string _baseDir = null;
        private static string BaseDir
        {
            get
            {
                if (_baseDir != null)
                    return _baseDir;

                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var parts = baseDir.Split('\\');

                var sb = new StringBuilder();
                for (int i = 0; i < parts.Length - 3; i++)
                {
                    sb.Append(parts[i]);
                    sb.Append('\\');
                }

                _baseDir = sb.ToString();

                return _baseDir;
            }
        }

        private static XDocument _xdoc = null;
        private static XDocument XDoc
        {
            get
            {
                if (_xdoc != null)
                    return _xdoc;

                var fullPath = Path.Combine(BaseDir, "MetaSandbox.csproj");
                _xdoc = XDocument.Load(fullPath);

                return _xdoc;
            }
        }

        private static XElement _compileItemGroup = null;
        private static XElement CompileItemGroup
        {
            get
            {
                if (_compileItemGroup != null)
                    return _compileItemGroup;

                _compileItemGroup = XDoc.Descendants(XDoc.Root.GetDefaultNamespace() + "ItemGroup").Where(i => i.Descendants(i.GetDefaultNamespace() + "Compile").Any()).SingleOrDefault();

                return _compileItemGroup;
            }
        }

        static void Main(string[] args)
        {
            ProjectFileTest();
        }

        private static void ProjectFileTest()
        {
            var success = WriteClass("MetaSandbox", "TestClass");

            if (success)
                AddClassToProject("TestClass");
        }

        private static bool WriteClass(string classNamespace, string className)
        {
            var classStr =
                "using System;\nusing System.Collections.Generic;\nusing System.Linq;\nusing System.Text;\nusing System.Threading.Tasks;\n\nnamespace " +
                classNamespace + "\n{\n\tclass " + className + "\n\t{\n\t}\n}";

            var path = Path.Combine(BaseDir, className + ".cs");

            if (!File.Exists(path))
            {
                File.WriteAllText(path, classStr);
                return true;
            }

            return false;
        }

        private static void AddClassToProject(string className)
        {
            var classElem = new XElement(XDoc.Root.GetDefaultNamespace() + "Compile", new XAttribute("Include", className + ".cs"));
            CompileItemGroup.Add(classElem);
            XDoc.Save(Path.Combine(BaseDir, "MetaSandbox.csproj"));
        }

    }
}
