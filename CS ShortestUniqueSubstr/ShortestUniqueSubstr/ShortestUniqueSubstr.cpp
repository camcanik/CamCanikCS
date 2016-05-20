#include <iostream>
#include <iomanip>
#include <string>

using std::setfill;
using std::string;
using std::right;
using std::setw;
using std::left;
using std::cout;
using std::endl;
using std::size_t;

static string getShortestUniqueSubstring(
	const string list[], int n, int index
	)
{
	string word = list[index];
	bool found = false;
	int extraLetters = 0;
	
	for (int i = 0; i <= word.length(); i++)
	{
		extraLetters = i+1;
		for (int j = 0; j + extraLetters <= word.length(); j++)
		{
			string sub = word.substr(j, extraLetters);
			found = false;

			while (!found)
			{
				for (int k = 0; k < n; k++)
				{
					if (k == index)
						continue;

					size_t pos = list[k].find(sub);
					if (pos!=string::npos)
					{
						found = true;
						break;
					}
				}
				if (!found)
					return sub;
			}	
		}
	}
	return("");
}

static int getMaxLength(const string list[], int n)
{
	int max = 0;

	for (int i = 0; i < n; i++)
	{
		int length = list[i].size();

		if (length > max)
			max = length;
	}

	return(max);
}

int main()
{
	const static string list[] =
	{
		"abc",
		"bca",
		"abcd",
		"bdca",
		"abcde",
		"deadc",
		"aabbcc",
		"aaabbbccc",
		"abbcccdddd",
		"dccbbbaaaaa",
		"mnopqrstuv",
		"lmnopqrstu",
		"klmnopqrst",
		"zyxwvutsr",
		"yxwvutsrq",
		"aaaaabbbbb",
		"aeiou",
		"stuvwxyz",
		"rstuvwxyz",
		"qrstuvwxyz",
		"camiscool",
		"cplusplus",
		"yo"
	};

	const static int n = sizeof(list) / sizeof(list[0]);
	const static int length = getMaxLength(list, n);

	for (int i = 0; i < n; i++)
	{
		cout << setw(2) << right << (i + 1) << ": " << left << setw(length)
			<< list[i] << " " << "substring: \""
			<< getShortestUniqueSubstring(list, n, i) << "\"" << endl;
	}

	getchar();
	return(0);
}
