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

namespace Daemoniq.Core
{
    static class ThrowHelper
    {
        public static void ThrowArgumentNullIfNull(object o, string paramName)
        {
            if(o == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void ThrowArgumentOutOfRangeIf<T>(Predicate<T> predicate,
                                                        T value,
                                                        string paramName,
                                                        string message)
        {
            if (predicate(value))
            {
                throw new ArgumentOutOfRangeException(paramName, message);
            }
        }

        public static void ThrowInvalidOperationExceptionIf<T>(Predicate<T> predicate, 
                                                               T value, 
                                                               string message)
        {
            if (predicate(value))
            {
                throw new InvalidOperationException(message);
            }
        }

        public static void ThrowInvalidOperationExceptionIf<T>(Predicate<T> predicate,
                                                               T value,
                                                               string message, 
                                                               params object[] args)
        {
            if (predicate(value))
            {
                throw new InvalidOperationException(string.Format(message, args));
            }
        }   

        public static void ThrowArgumentOutOfRangeIf<T>(Predicate<T> predicate,
                                                        T value,
                                                        string paramName)
        {
            if (predicate(value))
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }  

        public static void ThrowArgumentOutOfRangeIfEmpty(string s, 
                                                          string paramName)
        {
            ThrowArgumentOutOfRangeIf(str=>string.IsNullOrEmpty(str), s, paramName);
        }

        public static void ThrowArgumentOutOfRangeIfEmpty(char c,
                                                          string paramName)
        {
            ThrowArgumentOutOfRangeIf(Char.IsWhiteSpace, c, paramName);
        }

        public static void ThrowArgumentOutOfRangeIfZero(int i,
                                                         string paramName)
        {
            ThrowArgumentOutOfRangeIf(num => num == 0, i, paramName,
                string.Format("Argument '{0}' must not be equal to '0'(zero) ", paramName));
        }
    }
}