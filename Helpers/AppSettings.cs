using System.Xml;

namespace NFK001
{
    /// <summary>
    /// Modelo de configurações do sistema.
    /// </summary>
    public class AppSettings
    {
        public bool Homologacao { get; set; }

        public Connections? Connections { get; set; }

        public Log? Log { get; set; }

        public string? FilePath { get; set; }

        private static string? ConnectionAux { get; set; }

        public static string ConnectionStringMRT
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(ConnectionAux)) return ConnectionAux;

                    XmlDocument xml = new();
                    xml.Load(Util.AppSettings.Connections.MRT001);
                    XmlNode node = xml.DocumentElement.SelectSingleNode("connectionStrings/add[@name='MRT001']");
                    ConnectionAux = node.Attributes["connectionString"].Value;

                    return ConnectionAux;
                }
                catch
                {
                    return null;
                }
            }
        }
    }

    /// <summary>
    /// Conexões do Sistema.
    /// </summary>
    public class Connections
    {
        public string MRT001 { get; set; }

    }

    /// <summary>
    /// Log do Sistema.
    /// </summary>
    public class Log
    {
        public string Path { get; set; }
        public string FilePrefix { get; set; }
    }
}