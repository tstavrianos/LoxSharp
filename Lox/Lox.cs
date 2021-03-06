﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoxLanguage
{
    public class Lox : IErrorHandler
    {
        private string[] m_Arguements;
        private bool m_HadError;
        private bool m_HasRuntimeError;
        private Interpreter m_Interpreter; 

        public Lox(string[] args)
        {
            m_Arguements = args;
            m_Interpreter = new Interpreter(this);
        }

        public int Run()
        {
            if (m_Arguements.Length == 1)
            {
                ReadFile(m_Arguements[0]);
            }
            else
            {
                RunPrompt();
            }

            if(m_HadError)
            {
                return 60;
            }

            if(m_HasRuntimeError)
            {
                return 70;
            }
            return 0;
        }

        private void RunPrompt()
        {
            do
            {
                // Write the input mark
                Console.Write(">");
                // Read the input
                string voxCode = Console.ReadLine();
                // Run it
                Execute(voxCode);

            } while (!ShouldEscape());
        }

        /// <summary>
        /// Should we escape the prompt?
        /// </summary>
        /// <returns></returns>
        private bool ShouldEscape()
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            return key.Key == ConsoleKey.C && key.Modifiers == ConsoleModifiers.Control;
        }

        private void ReadFile(string path)
        {
            // Read the text
            if (!File.Exists(path))
            {
                Debug.LogError("Lox file could not be found at path {0}", path);
                return;
            }
            // Load the text
            string voxScript = File.ReadAllText(path);
            // Run it.
            Execute(voxScript);
        }

        public void Report(int line, string where, string message)
        {
            Debug.LogError("[line " + line + ":" + where + "] " + message);
            m_HadError = true;
        }

        /// <summary>
        /// Takes a string of VOX code and executes it.
        /// </summary>
        private void Execute(string voxSource)
        {
            // Used to parse our source.
            Scanner scanner = new Scanner(voxSource, this);
            // create a list of tokens
            List<Token> tokens = scanner.ScanTokens();
            // Create our parser
            Parser parser = new Parser(tokens, this);
            // Escape if we had errors
            if (m_HadError) return;
            // Start the parse process.
            List<Stmt> statements = parser.Parse();
            // Escape if there were errors
            if (m_HadError) return;
            // Create our resolver
            Resolver resolver = new Resolver(m_Interpreter, this);
            // Start resolving
            resolver.Resolve(statements);
            // Escape if we had errors
            if (m_HadError) return;
            // Start our interpreter
            m_Interpreter.Interpret(statements); 
        }

        public void Error(int line, string message)
        {
            m_HadError = true;
            Debug.LogError("Line:{0} {1}", line, message);
        }

        public void Error(Token token, string message)
        {
            m_HadError = true;
            if (token.type == TokenType.EOF)
            {
                Report(token.line, " at end", message);
            }
            else
            {
                Report(token.line, " at '" + token.lexeme + "'", message);
            }
        }

        /// <summary>
        /// Invoked when we get a runtime error while running the interpreter
        /// </summary>
        public void RuntimeError(RuntimeError error)
        {
            Debug.LogError(error.Message);
            Debug.LogError("[line " + error.token.line + "]");
            m_HasRuntimeError = true;
        }
    }
}
