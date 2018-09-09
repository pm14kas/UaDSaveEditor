using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Text.RegularExpressions;

namespace uad_save_editor
{
    class Program
    {

        [StructLayout(LayoutKind.Explicit)]
        struct Double2ulong
        {
            [FieldOffset(0)]
            public double d;
            [FieldOffset(0)]
            public ulong ul;
        }
        class SaveGame
        {
            const int size = 43;
            public string[] storage = new string[size];

            public SaveGame()
            {
                storage = new string[size];
            }
            public SaveGame(string decoded)
            {
                storage[0] = decoded.Substring(0, 20);// "header");
                for (int i = 0; i < size - 1; i++)
                {
                    storage[i + 1] = decoded.Substring(20 + i * 24, 24);// + "];);
                }
            }

            public override string ToString()
            {
                string result = "";
                foreach(string i in this.storage)
                {
                    result = result + i;
                }

                return result;
            }

            public double getDouble(int counter)
            {
                string temp = workWithEndians(this.storage[counter]);

                Double2ulong d2ul = new Double2ulong();
                string hex_input = temp.Substring(0, 16);
                ulong parsed = ulong.Parse(hex_input, NumberStyles.AllowHexSpecifier);
                d2ul.ul = parsed;

                return d2ul.d;
            }

            public string setDouble(double value, int counter)
            {
                if (counter == 0)
                {
                    return this.storage[counter];
                }

                Double2ulong d2ul = new Double2ulong();
                d2ul.d = value;

                string conversion = string.Format("{0:X}", d2ul.ul);

                if (d2ul.d == 0)
                {
                    conversion = "0000000000000000";
                }
                
                conversion = conversion + workWithEndians(this.storage[counter]).Substring(16);

                this.storage[counter] = workWithEndians(conversion);

                return this.storage[counter];
            }

            public string getString(int counter)
            {
                var bytes = new byte[this.storage[counter].Length / 2];
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(this.storage[counter].Substring(i * 2, 2), 16);
                }

                return Encoding.UTF8.GetString(bytes);
            }

            public string setString(string value, int counter)
            {
                //Still under development
                return this.storage[counter];
            }
        }
    
        /**
         * main method
         * 
         */
        static void Main(string[] args)
        {
            string decoded = loadGameFile();

            SaveGame save = new SaveGame(decoded);
            for (int i = 0; i < save.storage.Length; i++)
            {
                save.setDouble(save.getDouble(i), i);
                save.setString(save.getString(i), i);
            }
            string testString = save.ToString();
            //sort of living tests just in case, will definitely cause problems in the wild
            if (decoded != testString)
            {
                Console.WriteLine("Save file structure has changed, result is not guaranteed. Press any key to continue editing");
                Console.ReadKey(false);
            }

            Console.WriteLine("Up and Down save editor");
            Console.WriteLine("Levels: " + save.getDouble(2));
            Console.WriteLine("Gems: " + save.getDouble(6) + "   Gems3: " + save.getDouble(7));
            Console.WriteLine("Gems4: " + save.getDouble(8) + "   Gems4s: " + save.getDouble(9));
            Console.WriteLine("Gems5: " + save.getDouble(10) + "   Gems6: " + save.getDouble(11));
            Console.WriteLine("Gems7: " + save.getDouble(12) + "   Gems7s: " + save.getDouble(13));
            Console.WriteLine("Gems8: " + save.getDouble(14) + "   Gold: " + save.getDouble(15));
            Console.WriteLine();
            Console.WriteLine("Input variable and its new value f.e. \"gems7 500\" will set gems7 to 500. Input \"exit\" to save and exit, input \"refresh\" to show all data.");

            bool exit = false;
            while (!exit)
            {
                string input = Console.ReadLine().Trim();
                if (input == "exit")
                {
                    exit = true;
                }
                else if (input == "refresh")
                {
                    Console.WriteLine("Levels: " + save.getDouble(2));
                    Console.WriteLine("Gems1: " + save.getDouble(6) + "   Gems2: " + save.getDouble(7));
                    Console.WriteLine("Gems3: " + save.getDouble(8) + "   Gems4: " + save.getDouble(9));
                    Console.WriteLine("Gems5: " + save.getDouble(10) + "   Gems6: " + save.getDouble(11));
                    Console.WriteLine("Gems7: " + save.getDouble(12) + "   Gems8: " + save.getDouble(13));
                    Console.WriteLine("Gems9: " + save.getDouble(14) + "   Gold: " + save.getDouble(15));
                }
                else
                {
                    try
                    {
                        input = input.Trim();
                        MatchCollection vars = Regex.Matches(input, @"\b(\w+)\s+\b(\d+)");
                        string variable = vars[0].Groups[1].Value;
                        double value = Convert.ToDouble(vars[0].Groups[2].Value);

                        switch (variable)
                        {
                            case "levels":
                                save.setDouble(value, 2);
                                Console.WriteLine("'Levels' is now set to " + value.ToString());
                                break;
                            case "gems1":
                                save.setDouble(value, 6);
                                Console.WriteLine("'Gems1' is now set to " + value.ToString());
                                break;
                            case "gems2":
                                save.setDouble(value, 7);
                                Console.WriteLine("'Gems2' is now set to " + value.ToString());
                                break;
                            case "gems3":
                                save.setDouble(value, 8);
                                Console.WriteLine("'Gems3' is now set to " + value.ToString());
                                break;
                            case "gems4":
                                save.setDouble(value, 9);
                                Console.WriteLine("'Gems4' is now set to " + value.ToString());
                                break;
                            case "gems5":
                                save.setDouble(value, 10);
                                Console.WriteLine("'Gems5' is now set to " + value.ToString());
                                break;
                            case "gems6":
                                save.setDouble(value, 11);
                                Console.WriteLine("'Gems6' is now set to " + value.ToString());
                                break;
                            case "gems7":
                                save.setDouble(value, 12);
                                Console.WriteLine("'Gems7' is now set to " + value.ToString());
                                break;
                            case "gems8":
                                save.setDouble(value, 13);
                                Console.WriteLine("'Gems8' is now set to " + value.ToString());
                                break;
                            case "gems9":
                                save.setDouble(value, 14);
                                Console.WriteLine("'Gems9' is now set to " + value.ToString());
                                break;
                            case "gold":
                                save.setDouble(value, 15);
                                Console.WriteLine("'Gold' is now set to " + value.ToString());
                                break;
                            default:
                                Console.WriteLine("Variable not found");
                                throw new Exception();
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Invalid input");
                    }
                }
            }
            Console.WriteLine("Saving file...");
            saveGameFile(save.ToString());
            Console.WriteLine("File saved! Press any key to close the program");
            Console.ReadKey();

        }

        static char[] getKey()
        {
            //appears to be "Хуй" in some strange encoding
            char[] key = new char[3];
            key[0] = (char)202;
            key[1] = (char)79;
            key[2] = (char)44;
            return key;
        }

        static string workWithEndians(string param)
        {
            string temp = "";
            for (int j = param.Length - 1; j > 0; j -= 2)
            {
                temp = temp + param[j - 1] + param[j];
            }

            return temp;
        }

        static string loadGameFile(string path = "savegame")
        {
            try
            {
                char[] key = getKey();
                byte[] file = File.ReadAllBytes("savegame");
                StringBuilder str = new StringBuilder();
                int i = 0;
                foreach (byte b in file)
                {
                    str.Append((char)(b ^ key[i % 3]));
                    i++;
                }
                string cut = str.ToString().Substring(0, str.ToString().Length - 32);
                byte[] bd = Convert.FromBase64String(cut);
                StringBuilder dstr = new StringBuilder(bd.Length * 2);
                foreach (byte b in bd)
                {
                    dstr.Append((char)b);
                }

                return dstr.ToString();
            }
            catch
            { 
                Console.WriteLine("Error reading savegame file");
                Console.ReadKey(false);
                Environment.Exit(0);
                return "";
            }
        }

        static void saveGameFile(string str, string path = "savegame_edited")
        {
            char[] key = getKey();

            str = Convert.ToBase64String(Encoding.UTF8.GetBytes(str));

            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] hash = md5.ComputeHash(Encoding.Unicode.GetBytes(str));

            StringBuilder sh = new StringBuilder();
            foreach (byte b in hash)
            {
                sh.Append(b.ToString("x2"));
            }

            str = str + sh.ToString();
            using (System.IO.FileStream file = File.OpenWrite(path))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    file.WriteByte((byte)(str[i] ^ key[i % 3]));
                }
                file.Close();
            }
        }
    }
}
