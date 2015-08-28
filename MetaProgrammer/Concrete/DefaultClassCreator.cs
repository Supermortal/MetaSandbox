using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MetaProgrammer.Abstract;

namespace MetaProgrammer.Concrete
{
    public class DefaultClassCreator : IClassCreator
    {

        public void CreateClass(string classNamespace, string className)
        {
            var baseDir = GetBaseDirectory();
            var projectName = GetProjectName();
            var xdoc = GetProjectXDocument(baseDir, projectName);            
            
            var success = WriteClass(classNamespace, className, baseDir);

            if (success)
                AddClassToProject(className, baseDir, projectName, xdoc);
        }

        public bool WriteClass(string classNamespace, string className, string baseDir)
        {
            var classStr =
                "using System;\nusing System.Collections.Generic;\nusing System.Linq;\nusing System.Text;\nusing System.Threading.Tasks;\n\nnamespace " +
                classNamespace + "\n{\n\tclass " + className + "\n\t{\n\t}\n}";

            var path = Path.Combine(baseDir, className + ".cs");

            if (!File.Exists(path))
            {
                File.WriteAllText(path, classStr);
                return true;
            }

            return false;
        }

        public void AddClassToProject(string className, string baseDir, string projectName, XDocument projectXDoc)
        {
            var compileItemGroup = GetProjectCompileItemGroup(projectXDoc);

            var classElem = new XElement(projectXDoc.Root.GetDefaultNamespace() + "Compile", new XAttribute("Include", className + ".cs"));
            compileItemGroup.Add(classElem);
            projectXDoc.Save(Path.Combine(baseDir, projectName + ".csproj"));
        }

        public string GetProjectName()
        {
            var fileName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
            var parts = fileName.Split('.');
            return parts[0];
        }

        public string GetBaseDirectory()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var parts = baseDir.Split('\\');

            var sb = new StringBuilder();
            for (int i = 0; i < parts.Length - 3; i++)
            {
                sb.Append(parts[i]);
                sb.Append('\\');
            }

            return sb.ToString();
        }

        public XDocument GetProjectXDocument(string baseDir, string projectName)
        {
            var fullPath = Path.Combine(baseDir, projectName + ".csproj");
            return XDocument.Load(fullPath);
        }

        public XElement GetProjectCompileItemGroup(XDocument projectXDoc)
        {
            return projectXDoc.Descendants(projectXDoc.Root.GetDefaultNamespace() + "ItemGroup").Where(i => i.Descendants(i.GetDefaultNamespace() + "Compile").Any()).SingleOrDefault();
        }

    }
}
