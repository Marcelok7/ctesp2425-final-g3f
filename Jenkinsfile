pipeline {
    agent any

    environment {
        VAGRANT_DIR = "RestauranteFinal/Vagrantfile"
        DATABASE_URL = "jdbc:mysql://192.168.1.164:3306/Reservations"
        DATABASE_USER = "sa"
        DATABASE_PASSWORD = "Teste123!"
        DOCKER_IMAGE = "restaurante-api"
    }

    stages {
        stage('Iniciar Vagrant') {
            steps {
                script {
                    dir(env.VAGRANT_DIR) {
                        sh 'vagrant up --provision'
                    }
                }
            }
        }

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
