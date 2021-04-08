using System;

public class SpellWordAttribute : Attribute
{
	private string m_spellWord;
	public string SpellWord
	{
		get { return m_spellWord; }
	}

	public SpellWordAttribute(string word)
	{
		m_spellWord = word;
	}
}