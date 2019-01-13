FROM microsoft/dotnet:2.1-sdk

ARG branch=master

RUN apt-get update
RUN apt-get install -y unzip libunwind8 gettext
RUN mkdir NewsCrawler
ADD https://github.com/RichTeaMan/NewsCrawler/archive/$branch.tar.gz newsCrawler.tar.gz
RUN tar -xzf newsCrawler.tar.gz --strip-components=1 -C NewsCrawler
WORKDIR /NewsCrawler
RUN ./cake.sh -target=build
ENTRYPOINT ./cake.sh -target=Run
