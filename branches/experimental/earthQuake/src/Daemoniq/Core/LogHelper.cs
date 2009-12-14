/*
 *  Copyright 2009 Kriztian Jake Sta. Teresa
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Daemoniq.Core
{
    public static class LogHelper
    {
        private static readonly Dictionary<int, string> threadToIndentMap = 
            new Dictionary<int, string>();
        
        private static string buildstring(string strMessage)
        {
            string indentString;
            int hashCode = Thread.CurrentThread.GetHashCode();
            if (threadToIndentMap.ContainsKey(hashCode))
            {
                indentString = threadToIndentMap[hashCode];
            }
            else
            {
                indentString = string.Empty;
                threadToIndentMap.Add(hashCode, indentString);
            }
            return string.Format("{0} {1}[{2}] {3}", new object[] { DateTime.Now.ToString("HH:mm:ss"), indentString, hashCode, strMessage });
        }

        public static void EnterFunction()
        {
            var stackFrame = new StackFrame(1);
            WriteLine(">{0}.{1}", 
                stackFrame.GetMethod().DeclaringType.Name,
                stackFrame.GetMethod().Name);
            Indent();
        }

        public static void EnterFunction(params object[] args)
        {
            var stackFrame = new StackFrame(1);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(">{0}.{1}",
                                       stackFrame.GetMethod().DeclaringType.Name,
                                       stackFrame.GetMethod().Name);
            stringBuilder.Append("( ");
            for (int i = 0; i < args.Length; i++)
            {
                object arg = args[0];
                stringBuilder.Append(arg);
                if(i != args.Length -1)
                {
                    stringBuilder.Append(", ");
                }
            }
            stringBuilder.Append(" )");
            WriteLine(stringBuilder.ToString());
            Indent();
        }

        public static void LeaveFunction()
        {
            Unindent();
            var stackFrame = new StackFrame(1);
            WriteLine("<" + (stackFrame.GetMethod().DeclaringType.Name + "." + stackFrame.GetMethod().Name));
        }

        public static void Indent()
        {
            int hashCode = Thread.CurrentThread.GetHashCode();
            string indentString = threadToIndentMap[hashCode] ?? "";
            indentString = indentString + new string(' ', Trace.IndentSize);
            threadToIndentMap[hashCode] = indentString;
        }

        public static void Unindent()
        {
            int hashCode = Thread.CurrentThread.GetHashCode();
            string indentString = string.Empty;
            if (threadToIndentMap.ContainsKey(hashCode))
            {
                indentString = threadToIndentMap[hashCode];
            }
            if (indentString.Length >= Trace.IndentSize)
            {
                indentString = indentString.Remove(0, Trace.IndentSize);
            }
            threadToIndentMap[hashCode] = indentString;
        }

        public static void WriteLine(string strMessage)
        {
            Trace.WriteLine(buildstring(strMessage));
        }

        public static void WriteLine(string strMessage, params object[] args)
        {
            Trace.WriteLine(string.Format(buildstring(strMessage), args));
        }

        public static void WriteLineIf(bool bCondition, string strMessage)
        {
            if (bCondition)
            {
                WriteLine(strMessage);
            }
        }

        public static void WriteLineIf(bool bCondition, string strMessage, params object[] args)
        {
            if (bCondition)
            {
                WriteLine(strMessage, args);
            }
        }

        public static void Error(Exception e)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{2}{0}Exception.Source: {1}{2}", new string(' ', 3), e.Source, Environment.NewLine);
            stringBuilder.AppendFormat("{0}Exception.Message: {1}{2}", new string(' ', 3), e.Message, Environment.NewLine);
            stringBuilder.AppendFormat("{0}Exception.StackTrace: {1}{2}", new string(' ', 3), e.StackTrace, Environment.NewLine);
            if (e.InnerException != null)
            {
                stringBuilder.AppendFormat("{0}Exception.InnerException: {1}", new string(' ', 3), e.InnerException);
            }
            WriteLine(stringBuilder.ToString());
        }
    }
}
