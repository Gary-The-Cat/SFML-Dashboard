using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabaIsYou.Enums.Nodes;
public static class NodeFunctions
{
    public static bool IsNotSet(this Node node) => node.Match(
            not_set => true,
            noun => false,
            adjective => false,
            conjunction => false,
            objectNode => false);

    public static bool IsType(this Node node, Node b) => node.Match(
            not_set => b.IsT0 && b.AsT0 == not_set,
            noun =>  b.IsT1 && b.AsT1 == noun,
            adjective => b.IsT2 && b.AsT2 == adjective,
            conjunction => b.IsT3 && b.AsT3 == conjunction,
            objectNode => b.IsT4 && b.AsT4 == objectNode);
}
