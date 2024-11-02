/*
	Add as AUTOLOAD
	
	Intended to be used with an Excel spreadsheet that contains dialogue that will be parsed and stored in memory.
	Excel columns should have this format:
	A: dialogue_key; a unique key that identifies the dialogue text/node. i.e. hailey_intro_speech_1
	B: node_type; text or question
	C: next_node_key; 1 or more dialogue keys that identifies the next dialogue. "end" is used to indicate end of dialgoue.
		i.e. hailey_intro_speech_2, hailey_angry_speech_1, end
	D: icon_key; identifies the icon that should be used for the dialogue. i.e. hailey_happy_face
		Use "none" to indicate no icon
	E: ##_text; the actual text to be displayed. This can be the text for a dialogue, question, or question answer.
		i.e. Hailey is very happy to be with you.
		Each row from E...etc is used for different langauge localization. E is English, G is Spanish, etc.
		## indicates the language.
		i.e. english_text
*/

using Godot;

public partial class DialogueManager : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
