dotnet build
dotnet test @coverage.rsp
dotnet tool install coveralls.net --version 1.0.0 --tool-path tools
./tools/csmacnz.Coveralls --opencover -i cover.xml --useRelativePaths \
--commitId $TRAVIS_COMMIT --commitBranch $TRAVIS_BRANCH --commitAuthor \
$AUTHOR_NAME --commitEmail $COMMITTER_EMAIL --commitMessage $TRAVIS_COMMIT_MESSAGE \
--jobId $TRAVIS_BUILD_NUMBER --serviceName travis
