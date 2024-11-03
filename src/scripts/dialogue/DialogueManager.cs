/*
	Add as AUTOLOAD
	

	textDict will contain all the in-game text using a key-value pair.
	The key will be the text's unique ID.
	The value will be the text in the appropriate language, selected in the settings.

	Text will be read from a tab separated value (.tsv) file that contains all the texts with their unique ID and translations.
	Example:
	dialogue_key	english_text	spanish_text
	greeting_1	hello	hola
	bye_1	bye	adios

	Use an Google spreadsheet to organize these text and download it as a .tsv file.
	
	DO NOT EDIT TSV FILE IN VS CODE. TABS ARE NOT INSERTED CORRECTLY. EDIT IN GOOGLE SHEETS AND DOWNLOAD AS TSV.
*/

using Godot;
using System.Collections.Generic;
using System.IO;

public partial class DialogueManager : Node
{
	private const string _textFilePath = "src/resources/text.tsv";
	private string _language = "english_text"; // Move this to a Settings.cs file later
	private Dictionary<string, string> _textDict = new Dictionary<string, string>(); // Move this to a another class?
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("DialogueManager added.");
		ReadTextFromFile();
	}

	private void ReadTextFromFile() {
		if (!File.Exists(_textFilePath)) {
			throw new IOException("Dialogue file cannot be found.");
		}
		GD.Print("Dialogue file found.");

		_textDict.Clear(); 

		using (StreamReader reader = new StreamReader(_textFilePath)) {
			string line = reader.ReadLine();
			string[] columnHeaders = line.Split("\t");
			int translationIndex = 0;

			for (int i = 1; i < columnHeaders.Length; i++) {
				if (columnHeaders[i].Trim().Equals(_language)) {
					translationIndex = i;
					break;
				}
			}

			if (translationIndex == 0) {
				throw new IOException("Text for selected language not found.");
			}

			while ((line = reader.ReadLine()) != null) {
				GD.Print(line);
				string[] cells  = line.Split("\t");
				_textDict.Add(cells[0].Trim(), cells[translationIndex].Trim());
			}
		}
	}
}
