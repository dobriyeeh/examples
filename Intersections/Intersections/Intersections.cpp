#include <iostream>
#include "Intersections.h"

int main()
{
	Lines lines =
	{
		Line("red", { "a", "b", "c", "d" }),
		Line("blue", {"b", "c"}),
		Line("green", {"c"}),
		Line("yellow", {"d"}),
	};

	Navigator navigator;

	auto transitions = navigator.FindIntersections(lines);

	for (const auto& transition : transitions)
	{
		std::cout << transition.first.c_str() << " with " << transition.second.size() << std::endl;
	}
}
