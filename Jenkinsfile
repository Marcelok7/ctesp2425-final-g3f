pipeline {
    agent any

    environment {
        DOCKER_IMAGE = "restaurante-api"
    }

    stages {
        stage('Checkout do Código') {
            steps {
                git branch: 'main', url: 'https://github.com/Marcelok7/ctesp2425-final-g3f.git'
            }
        }

        stage('Compilação e Testes') {
            steps {
                sh 'mvn clean package'
                sh 'mvn test'
            }
        }

        stage('Análise de Qualidade com SonarQube') {
            steps {
                withSonarQubeEnv('SonarQube') {
                    sh 'mvn sonar:sonar'
                }
            }
        }

        stage('Construção da Imagem Docker') {
            steps {
                sh 'docker build -t $DOCKER_IMAGE .'
            }
        }

        stage('Deploy com Docker') {
            steps {
                sh '''
                docker-compose down
                docker-compose up -d
                '''
            }
        }
    }

    post {
        success {
            echo 'Pipeline executado com sucesso!'
        }
        failure {
            echo 'Erro no pipeline!'
        }
    }
}