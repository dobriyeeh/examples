#include <iostream>
#include <vector>
#include <iterator>
#include <array>

template <typename T, typename P>
std::pair<typename T::const_iterator, typename T::const_iterator> GetMaxSequence(const T& container, P predicate)
{
	if (std::begin(container) == std::end(container))
	{
		return std::make_pair(std::end(container), std::end(container));
	}

	typename T::const_iterator maxSeqBegin;
	typename T::const_iterator maxSeqEnd;
	size_t maxSeqLength = 0;

	typename T::const_iterator workingPrev = std::begin(container);
	typename T::const_iterator workingSeqBegin = std::begin(container);
	typename T::const_iterator workingSeqEnd;

	for (workingSeqEnd = std::begin(container) + 1; workingSeqEnd != std::end(container); ++workingSeqEnd)
	{
		if (!predicate(*workingPrev, *workingSeqEnd))
		{
			size_t currLength = workingSeqEnd - workingSeqBegin;
			if (currLength > maxSeqLength)
			{
				maxSeqBegin = workingSeqBegin;
				maxSeqEnd = workingSeqEnd;
				maxSeqLength = currLength;

				workingSeqBegin = workingSeqEnd;
			}
		}

		workingPrev = workingSeqEnd;
	}

	int lastLength = workingSeqEnd - workingSeqBegin;
	if (lastLength > maxSeqLength)
	{
		maxSeqBegin = workingSeqBegin;
		maxSeqEnd = workingSeqEnd;
		maxSeqLength = lastLength;
	}

	return std::make_pair(maxSeqBegin, maxSeqEnd);
}

template <typename T>
std::pair<typename T::const_iterator, typename T::const_iterator> GetMaxSequence(const T& container)
{
	return GetMaxSequence(container, [](auto a, auto b) { return a < b; });
}

template <typename T>
void PrintLongestSequence(const T& container)
{
	auto result = GetMaxSequence(container);

	for (auto item = result.first; item < result.second; ++item)
	{
		std::cout << *item << std::endl;
	}
}

template <class T, size_t N>
void PrintLongestSequence(T (&array)[N])
{
	return PrintLongestSequence(array);
}

int main()
{
	std::vector<int> container = { 5, 6, 7, 1, 3, 4, 5, 3, 4, 6, 7, 8, 9, 10};
	PrintLongestSequence(container);

	std::array<char, 5> arr1 = { 'a', 'b', 'z', 'd', 'a' };
	PrintLongestSequence(arr1);
}
