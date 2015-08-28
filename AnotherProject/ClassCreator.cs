using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MetaProgrammer
{
    public class ClassCreator
    {
        //private string ProjectName { get; set; }

        private string _projectName = null;
        private string ProjectName
        {
            get
            {
                if (_projectName != null)
                    return _projectName;

                var fileName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
                var parts = fileName.Split('.');
                _projectName = parts[0];

                return _projectName;
            }
        }

        private string _baseDir = null;
        private string BaseDir
        {
            get
            {
                if (_baseDir != null)
                    return _baseDir;

                //var baseDir = AppDomain.CurrentDomain.BaseDirectory;

                //var index = baseDir.IndexOf(ProjectName, StringComparison.InvariantCulture);
                //_baseDir = baseDir.Substring(0, index);

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

        private XDocument _xdoc = null;
        private XDocument XDoc
        {
            get
            {
                if (_xdoc != null)
                    return _xdoc;

                var fullPath = Path.Combine(BaseDir, ProjectName + ".csproj");
                _xdoc = XDocument.Load(fullPath);

                return _xdoc;
            }
        }

        private XElement _compileItemGroup = null;
        private XElement CompileItemGroup
        {
            get
            {
                if (_compileItemGroup != null)
                    return _compileItemGroup;

                _compileItemGroup = XDoc.Descendants(XDoc.Root.GetDefaultNamespace() + "ItemGroup").Where(i => i.Descendants(i.GetDefaultNamespace() + "Compile").Any()).SingleOrDefault();

                return _compileItemGroup;
            }
        }

        public void CreateClass(string classNamespace, string className)
        {
            var success = WriteClass(classNamespace, className);

            if (success)
                AddClassToProject(className);
        }

        private bool WriteClass(string classNamespace, string className)
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

        private void AddClassToProject(string className)
        {
            var classElem = new XElement(XDoc.Root.GetDefaultNamespace() + "Compile", new XAttribute("Include", className + ".cs"));
            CompileItemGroup.Add(classElem);
            XDoc.Save(Path.Combine(BaseDir, ProjectName + ".csproj"));
        }
    }
}
