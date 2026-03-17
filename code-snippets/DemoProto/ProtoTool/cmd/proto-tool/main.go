package main

import (
	"flag"
	"fmt"
	"os"

	"github.com/emicklei/proto"
)

func main() {
	protoFile := flag.String("proto", "", "path to .proto file")
	flag.Parse()

	if *protoFile == "" {
		fmt.Fprintln(os.Stderr, "error: -proto flag is required")
		flag.Usage()
		os.Exit(1)
	}

	f, err := os.Open(*protoFile)
	if err != nil {
		fmt.Fprintf(os.Stderr, "error: cannot open file: %v\n", err)
		os.Exit(1)
	}
	defer f.Close()

	parser := proto.NewParser(f)
	definition, err := parser.Parse()
	if err != nil {
		fmt.Fprintf(os.Stderr, "error: cannot parse proto file: %v\n", err)
		os.Exit(1)
	}

	proto.Walk(definition,
		proto.WithService(func(s *proto.Service) {
			fmt.Printf("service %s:\n", s.Name)
			for _, element := range s.Elements {
				rpc, ok := element.(*proto.RPC)
				if !ok {
					fmt.Printf("  element %#v\n\n", element)
					continue
				}
				fmt.Printf("  rpc %s\n", rpc.Name)
				fmt.Printf("    request:  %s\n", rpc.RequestType)
				fmt.Printf("    response: %s\n", rpc.ReturnsType)
				fmt.Printf("    detail: %#v\n", rpc)
				for _, item := range rpc.Options {
					fmt.Printf("        %#v\n", item)
				}
			}
		}),
	)
}
