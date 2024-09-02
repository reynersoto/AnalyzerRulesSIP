using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AnalyzerTests.Test.CSharpCodeFixVerifier<
    AnalyzerTests.LineAnalyzer,
    AnalyzerTests.AnalyzerTestsCodeFixProvider>;
using VerifyCS2 = AnalyzerTests.Test.CSharpCodeFixVerifier<
    AnalyzerTests.MethodAnalyzer,
    AnalyzerTests.AnalyzerTestsCodeFixProvider>;

namespace AnalyzerTests.Test
{
    [TestClass]
    public class AnalyzerTestsUnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"class C
{
    void M(int valor)
    { 
        //1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
        //2
        //3
        //4
        //5
        //6
    }
    void N(int valor)
    { 
            //1
            //2
            //3
            //4
            //5
            //6
    }
void O(int valor)
    {  
            //1
            valor = 1; //2
            if (valor == 1) //3
            {               //4
                valor=2;    //5
            }               //6
            //7
            //8
            //9
            //10
            //11
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TestMethod2()
        {

            var test = @"class C
{
    void M(int valor)
    { 
        //1
        //2
        //3
        //4
        //5
        //6
    }
    void N(int valor)
    { 
            //1
            //2
            //3
            //4
            //5
            //6
    }
void O(int valor)
    {  
            //1
            valor = 1; //2
            if (valor == 1) //3
            {               //4
                valor=2;    //5
            }               //6
            //7
            //8
            //9
            //10
            //11
    }
}";
            await VerifyCS2.VerifyAnalyzerAsync(test);
        }
    }
}
