#pragma once

#include <vector>
#include <unordered_set>
#include <unordered_map>
#include <algorithm>

using LineStations = std::vector<std::string>;

class Line
{
public:

	Line(const std::string& name, LineStations lineStations)
	{
		_name = name;
		_lineStations = lineStations;
	}

	const std::string& GetName() const
	{
		return _name;
	}

	const LineStations& GetLineStations() const
	{
		return _lineStations;
	}

private:
	std::string _name;
	LineStations _lineStations;
};

using Lines = std::vector<Line>;
using Transitions = std::unordered_map<std::string, std::vector<std::string>>;
using TransitionsList = std::vector<std::pair<std::string, std::vector<std::string>>>;

class Navigator
{
public:

	TransitionsList FindIntersections(const Lines& lines)
	{	
		Transitions transitions;

		for (const auto& currLine : lines)
		{
			for (const auto& currStation : currLine.GetLineStations())
			{
				auto transitionIt = transitions.find(currStation);

				if (transitionIt != transitions.end())
				{
					transitionIt->second.push_back(currLine.GetName());
				}
				else
				{
					transitions[currStation] = { currLine.GetName() };
				}
			}
		}

		TransitionsList result;
		
		std::copy_if(
			transitions.begin(), 
			transitions.end(), 
			std::back_inserter(result),
			[](const auto& tr) { return tr.second.size() > 1; });

		return result;
	}
};
