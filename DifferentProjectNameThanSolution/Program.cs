using MetaProgrammer.Concrete;

namespace DifferentProjectNameThanSolution
{
    class Program
    {

        static void Main(string[] args)
        {
            var cc = new DefaultClassCreator();
            cc.CreateClass("DifferentProjectNameThanSolution", "TestClass");
        }

    }
}
