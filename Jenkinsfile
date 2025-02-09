node {
  stage('SCM') {
    checkout scm
  }
  stage('SonarQube Analysis') {
    def scannerHome = tool 'SonarScanner for MSBuild'
    withSonarQubeEnv() {
      sh "${scannerHome}/SonarScanner.MSBuild.exe begin /k:\"restaurante-api-5\""
      sh "dotnet build"
      sh "${scannerHome}/SonarScanner.MSBuild.exe end"
    }
  }
}
