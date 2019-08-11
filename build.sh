set -v

dotnet build

# code coverage generation settings are very long,
# they are moved to custrom .rsp file
dotnet test @coverage.rsp

# upload coverage
dotnet tool install coveralls.net --version 1.0.0 --tool-path tools
./tools/csmacnz.Coveralls --opencover -i cover.xml --useRelativePaths \
--commitId $TRAVIS_COMMIT --commitBranch "$TRAVIS_BRANCH" \
--commitAuthor "$AUTHOR_NAME" --commitEmail "$COMMITTER_EMAIL" \
--commitMessage "$TRAVIS_COMMIT_MESSAGE" \
--jobId "$TRAVIS_JOB_ID" --serviceName travis

set +v
