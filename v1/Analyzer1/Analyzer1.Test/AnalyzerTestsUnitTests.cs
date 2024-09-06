using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Analyzer1.Test.CSharpCodeFixVerifier<
    Analyzer1.LineAnalyzer,
    Analyzer1.CommentsCapitalCodeFixProvider>;
using VerifyCS2 = Analyzer1.Test.CSharpCodeFixVerifier<
    Analyzer1.MethodAnalyzer,
    Analyzer1.CommentsCapitalCodeFixProvider>;
using VerifyCS3 = Analyzer1.Test.CSharpCodeFixVerifier<
    Analyzer1.CommentIndependecyAnalyzer,
    Analyzer1.CommentsCapitalCodeFixProvider>;
using VerifyCS4 = Analyzer1.Test.CSharpCodeFixVerifier<
    Analyzer1.CommentEndsWithPeriodAnalyzer,
    Analyzer1.CommentsCapitalCodeFixProvider>;

namespace Analyzer1.Test
{
    [TestClass]
    public class AnalyzerTestsUnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"
                        using System;
                        using System.Collections.Generic;
                        using System.Linq;
                        using System.Text;
                        using System.Threading.Tasks;
                        using System.Diagnostics;

                        namespace ConsoleApplication1
                        {
                            class {|#0:TypeName|}
                            {   
                            //1
                            //2
                            //3
                            //4
                            //5
                            //6
                            //7
                            //8
                            //9
                            //10
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
        [TestMethod]
        public async Task TestMethod3()
        {
            var test = @"class C
{

void O(int valor)
    {  
            //1
            valor = 1; 
            if (valor == 1) 
            {               
//5
                valor=2;    
            }               
            //8
            //9
valor = 2;  
            //11

            //12
    }
}";

            await VerifyCS3.VerifyAnalyzerAsync(test);

        }
        [TestMethod]
        public async Task TestMethod4()
        {
            var test = @"
                        using System;
                        using System.Collections.Generic;
                        using System.Linq;
                        using System.Text;
                        using System.Threading.Tasks;
                        using System.Diagnostics;

                        namespace ConsoleApplication1
                        {
                            class {|#0:TypeName|}
                            {   
                            /* Prueba
                                de comentarios.*/
                            }
                        }";

            await VerifyCS4.VerifyAnalyzerAsync(test);
        }
    }
}
