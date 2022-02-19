using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.IO;
using System.Reflection;

namespace Mediatek86.controleur
{
    /// <summary>
    /// Enregistrer les logs dans un fichier txt
    /// </summary>
    public static class EcrireLogs
    {
       /// <summary>
       /// Enregistrer message dans un fichier txt
       /// </summary>
       /// <param name="logMessage"></param>
        public static void Enregistrer(string logMessage)
        {
            string m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                using (StreamWriter w = File.AppendText($"{m_exePath}\\log.txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Ecrire log dans le fichier txt
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="txtWriter"></param>
        private static void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("Erreur : ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                txtWriter.WriteLine(logMessage);
                txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
