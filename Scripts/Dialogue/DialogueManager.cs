/*
	Add as AUTOLOAD
	
	dialogueDict will contain all the in-game text using a key-value pair.
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
using System;
using System.Collections.Generic;
using System.IO;

public partial class DialogueManager : Node
{
	private const string textFilePath = "Resources/text.tsv";
	private string language = "english_text";
	private Dictionary<string, string> textDict = new Dictionary<string, string>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("DialogueManager added.");
		ReadTextFromFile();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void ReadTextFromFile() {
		if (!File.Exists(textFilePath)) {
			throw new IOException("Dialogue file cannot be found.");
		}
		GD.Print("Dialogue file found.");

		textDict.Clear(); 

		using (StreamReader reader = new StreamReader(textFilePath)) {
			string line = reader.ReadLine();
			string[] columnHeaders = line.Split("\t");
			int translationIndex = 0;

			for (int i = 1; i < columnHeaders.Length; i++) {
				if (columnHeaders[i].Trim().Equals(language)) {
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
				textDict.Add(cells[0].Trim(), cells[translationIndex].Trim());
			}
		}
	}
}
