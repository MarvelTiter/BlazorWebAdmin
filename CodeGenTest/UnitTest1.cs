using LogAopCodeGenerator;

namespace CodeGenTest
{
    [TestClass]
    public class UnitTest1
    {
        [Aspectable()]
        public interface IForTest
        {
            Task<int> ReturnTaskValueAsync();
            Task ReturnTaskAsync();
            void ReturnVoid();
            Task<List<int>> ReturnGenTaskListAsync();
        }



        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}