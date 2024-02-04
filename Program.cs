using System.Data.SQLite;
using CsvHelper;

class Program
{
    static void Main()
    {
        // Path to CSV file
        string pathCSVFile = "data.csv";

        // Verifica se o arquivo CSV existe
        if (!File.Exists(pathCSVFile))
        {
            Console.WriteLine("ACSV file not found.");
            return;
        }

        // List to store CSV data
        List<Contato> listaContacts;

        // Reads data from CSV and stores it in the list
        using (var reader = new StreamReader(pathCSVFile))
        using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
        {
            listaContacts = csv.GetRecords<Contato>().ToList();
        }

        // Path to SQLite database
        string caminhoBancoDados = "dados.db";

        // Create table in SQLite database if it does not exist
        using (var conexao = new SQLiteConnection($"Data Source={caminhoBancoDados};Version=3;"))
        {
            conexao.Open();

            using (var comando = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Contacts (Name TEXT, Telephone TEXT, Birthdate TEXT);", conexao))
            {
                comando.ExecuteNonQuery();
            }

            // Insert data into the table
            foreach (var contato in listaContacts)
            {
                using (var comando = new SQLiteCommand("INSERT INTO Contacts (Name, Telephone, Birthdate) VALUES (@Name, @Telephone, @Birthdate);", conexao))
                {
                    comando.Parameters.AddWithValue("@Name", contato.Name);
                    comando.Parameters.AddWithValue("@Telephone", contato.Telephone);
                    comando.Parameters.AddWithValue("@Birthdate", contato.Birthdate);
                    comando.ExecuteNonQuery();
                }
            }
        }

        // Reads data from the SQLite database and displays it
        using (var conexao = new SQLiteConnection($"Data Source={caminhoBancoDados};Version=3;"))
        {
            conexao.Open();

            using (var comando = new SQLiteCommand("SELECT * FROM Contacts;", conexao))
            using (var leitor = comando.ExecuteReader())
            {
                Console.WriteLine("Data read from SQLite base:");

                while (leitor.Read())
                {
                    Console.WriteLine($"Name: {leitor["Name"]}, Telephone: {leitor["Telephone"]}, Data de Nascimento: {leitor["Birthdate"]}");
                }
            }
        }
    }
}

// Class to represent CSV data
class Contato
{
    public string Name { get; set; }
    public string Telephone { get; set; }
    public string Birthdate { get; set; }
}
