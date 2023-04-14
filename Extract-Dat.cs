using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


public class Program
{
    static async Task Main()
    {
        await ExtractDat();
        string textos = Unir(DatText);
        Number(DatText);
        File.WriteAllText(Path.Combine("Texto.txt"), textos);
    }

    public class TextData // Adicione o modificador 'public' aqui
    {
        public int Position { get; set; }
        public string Text { get; set; }
    }

    static List<TextData> DatText = new List<TextData>();
    static List<int> DatInt = new List<int>();

    public static async Task ExtractDat()
    {
        // Nome do DAT que será extraido o Texto
        string arquivo = "0000.dat";
        // Localização do texto inicial do Texto
        int SKIP_POSITION = 10760;
        //Final do Texto
        int END_POSITION = 16902;
      
        long lastPosition = 0;
      
        using (FileStream stream = new FileStream(arquivo, FileMode.Open, FileAccess.Read))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                stream.Seek(SKIP_POSITION, SeekOrigin.Begin);

                while (true) // ler as primeiras 5 strings
                {
                    if ((int)stream.Position > END_POSITION) break;
                    //long Findlong = await FindIntValueInFile3(arquivo, (int)stream.Position, (int)lastPosition);

                  long Findlong = await FindIntValueInFile(arquivo, (int)stream.Position);
                    int newInt = (int)Findlong;
                    if(newInt == -1) {
                      Console.WriteLine("Valor não encontrado");
                    }
                    lastPosition = Findlong;
                  
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

                    TextData textData = new TextData { Position = newInt, Text = txt };
                    DatText.Add(textData);
                }
            }
        }

    }

    static async Task<long> FindIntValueInFile(string fileName, int searchValue)
    {
        
        using (FileStream stream = new FileStream(fileName, FileMode.Open))
        {
            int bufferSize = (int)stream.Length; // Define o tamanho do buffer como o tamanho do arquivo
            byte[] buffer = new byte[bufferSize];

            long position = 0;
            
            while (await stream.ReadAsync(buffer, 0, bufferSize) > 0)
            {
                for (int i = 0; i < bufferSize - sizeof(int) + 1; i++)
                {
                    int value = BitConverter.ToInt32(buffer, i);
                    if (value == searchValue)
                    {
                        return position + i;
                    }
                }
                position += bufferSize - sizeof(int) + 1;
                stream.Seek(-sizeof(int) + 1, SeekOrigin.Current);
            }
        }
        return -1; // Valor não encontrado
    }
  
  static async Task<long> FindByteValueInFile(string fileName, int searchValue)
{
    using (FileStream stream = new FileStream(fileName, FileMode.Open))
    {
        int bufferSize = (int)stream.Length; // Define o tamanho do buffer como o tamanho do arquivo
        byte[] buffer = new byte[bufferSize];

        long position = 0;

        while (await stream.ReadAsync(buffer, 0, bufferSize) > 0)
        {
            for (int i = 0; i < bufferSize - sizeof(int) + 1; i++)
            {
                byte[] bytes = new byte[sizeof(int)];
                Buffer.BlockCopy(buffer, i, bytes, 0, sizeof(int));

                // Complementa com zero bytes se o valor convertido em bytes não tiver 4 bytes
                if (bytes.Length < sizeof(int))
                {
                    byte[] complement = new byte[sizeof(int) - bytes.Length];
                    bytes = complement.Concat(bytes).ToArray();
                }

                int value = BitConverter.ToInt32(bytes, 0);
                if (value == searchValue)
                {
                    return position + i;
                }
            }
            position += bufferSize - sizeof(int) + 1;
            stream.Seek(-sizeof(int) + 1, SeekOrigin.Current);
        }
    }
    return -1; // Valor não encontrado
}

    public static string Unir(List<TextData> listaDeStrings)
    {
        string resultado = "";

        foreach (TextData td in listaDeStrings)
        {
            resultado += $"({td.Position})\n\"{td.Text}\"\n[{td.Text}]\n\n";
        }

        return resultado;
    }
      public static void Number(List<TextData> listaDeStrings)
    {


        foreach (TextData td in listaDeStrings)
        {
            Console.WriteLine(td.Position);
        }


    }
}

