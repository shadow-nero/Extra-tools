using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
      string DatText = ExtractDat();
      File.WriteAllText(Path.Combine("Texto.txt", DatText);
    }
  
  public static string ExtractDat(){
        // Nome do DAT que será extraido o Texto
        string arquivo = "0000.dat";
        // Localização do texto inicial do Texto
        int SKIP_POSITION = 10760;
        //Final do Texto
        int END_POSITION = 16902;

        List<string> Texto = new List<string>();
    
    using (FileStream stream = new FileStream(arquivo, FileMode.Open, FileAccess.Read))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                stream.Seek(SKIP_POSITION, SeekOrigin.Begin);

                while(true) // ler as primeiras 5 strings
                {
                    if((int)stream.Position > END_POSITION) break;
                    byte[] buffer = new byte[1024]; 
                    int bytesRead = 0;
                    while (true)
                    {
                        byte b = reader.ReadByte();
                        if (b == 0x00)
                        {
                            break;
                        }
                        buffer[bytesRead] = b;
                        bytesRead++;
                    }
                    string txt = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    
                    int skipZero = (bytesRead + 1) % 4;
                    if (skipZero != 0)
                    {
                        stream.Seek(4 - skipZero, SeekOrigin.Current);
                    }
                  Texto.Add(txt);
                }
            }
        }
    
   /* for(int i = 0; i < Texto.Count; i++){
      Console.WriteLine(Texto[i]);
    }*/
    string saida = Unir(Texto);
    Console.WriteLine(saida);
    return saida;
    }
          public static string Unir(List<string> listaDeStrings)
        {
            string resultado = "";

            foreach (string s in listaDeStrings)
            {
                resultado += $"\"{s}\"\n[{s}]\n\n";
            }

            return resultado;
        }
}
