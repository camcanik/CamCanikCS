#include <iostream>
#include <string>
#include <vector>
#include <algorithm>

using std::string;
using std::vector;
using std::cout;
using std::endl;
using std::for_each;

#define SZ(x) ( sizeof(x) / sizeof(x[0]) )

vector<string> swapVec(vector<string> revStr)
{
	int n = revStr.size() - 1;
	for (int i = 0; i<n; i++)
	{
		if (i == n)
			continue;

		string temp = revStr[i];
		revStr[i] = revStr[n];
		revStr[n] = temp;
		n--;
	}
	return revStr;
}
void outputVec(string s)
{
	cout << "" << s;
}
static bool isWhiteSpace(char c)
{
	return(isspace(c) ? true : false);
}

static vector<string> separate(const string &s)
{
	string result = "[";

	for (int n = s.size(), i = 0; i < n; i++)
	{
		if (i > 0 && isWhiteSpace(s[i - 1]) != isWhiteSpace(s[i]))
			result += "][";

		result += s[i];
	}

	result += "]";

	string delimiter = "]";
	int pos = 0;
	vector<string> revStr;
	string word = "";

	while ((pos = result.find(delimiter)) != string::npos)
	{
		word = result.substr(0, pos);
		revStr.push_back(word);
		result.erase(0, pos + delimiter.length());
	}

	
	for (auto i = revStr.begin(); i != revStr.end(); i++)
	{
		*i = i->substr(1);
	}
	
	return revStr;
}

int main()
{
	string list[] =
	{
		"   one two  three",
		"one two  three   ",
		"a bb  ccc   ccc  bb a",
		"      .      ",
		" 1  2   3   4",
		"   1  2 3",
		" 11  222   3333   44444",
		"we come in peace",
		"shoot to kill"
	};
	vector<string> reverseWords;
	int n = SZ(list);

	for (int i = 0; i < n; i++)
	{
		reverseWords = separate(list[i]);
		reverseWords=swapVec(reverseWords);

		for_each(reverseWords.begin(), reverseWords.end(), outputVec);
		cout << endl;
	}

	getchar();
	return(0);
}
