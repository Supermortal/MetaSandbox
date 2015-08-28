using System.Xml.Linq;

namespace MetaProgrammer.Abstract
{
    public interface IClassCreator
    {
        void CreateClass(string classNamespace, string className);
        bool WriteClass(string classNamespace, string className, string baseDir);
        void AddClassToProject(string className, string baseDir, string projectName, XDocument projectXDoc);
        string GetProjectName();
        string GetBaseDirectory();
        XDocument GetProjectXDocument(string baseDir, string projectName);
        XElement GetProjectCompileItemGroup(XDocument projectXDoc);
    }
}
