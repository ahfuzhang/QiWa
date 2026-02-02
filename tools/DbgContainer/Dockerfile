FROM mcr.microsoft.com/dotnet/sdk:10.0

RUN apt-get update \
 && apt-get install -y --no-install-recommends curl unzip clang lld binutils zlib1g-dev wget \
 && rm -rf /var/lib/apt/lists/*

RUN curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /usr/bin/vsdbg
WORKDIR /tmp/netcoredbg
RUN wget https://github.com/Samsung/netcoredbg/releases/download/3.1.3-1062/netcoredbg-linux-amd64.tar.gz \
 && tar -zxf netcoredbg-linux-amd64.tar.gz -C /usr/bin/

ENV PATH="/usr/bin/vsdbg:/usr/bin/netcoredbg:${PATH}"

