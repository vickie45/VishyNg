#!/bin/bash
# Complete Employee System Management Script
# This script provides easy commands to manage the entire system

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Helper functions
print_step() {
    echo -e "${BLUE}📋 $1${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_banner() {
    echo -e "${BLUE}"
    echo "╔══════════════════════════════════════════════════════════════╗"
    echo "║                    EMPLOYEE SYSTEM                           ║"
    echo "║              Complete Development Stack                      ║" 
    echo "╚══════════════════════════════════════════════════════════════╝"
    echo -e "${NC}"
}

# Check if Docker is running
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        print_error "Docker is not running. Please start Docker Desktop."
        exit 1
    fi
    print_success "Docker is running"
}

# Check if .NET 6 is installed
check_dotnet() {
    if ! command -v dotnet &> /dev/null; then
        print_error ".NET CLI not found. Please install .NET 6 SDK."
        exit 1
    fi
    
    local dotnet_version=$(dotnet --version)
    if [[ $dotnet_version < "6.0" ]]; then
        print_error ".NET version $dotnet_version found. Please install .NET 6 SDK."
        exit 1
    fi
    print_success ".NET $dotnet_version is ready"
}

# Check if Node.js is installed
check_node() {
    if ! command -v npm &> /dev/null; then
        print_error "Node.js/npm not found. Please install Node.js 18+."
        exit 1
    fi
    print_success "Node.js $(node --version) is ready"
}

# Setup infrastructure
setup_infrastructure() {
    print_step "Starting infrastructure services (PostgreSQL, Kafka, Zookeeper)..."
    
    docker compose up -d postgres kafka zookeeper
    
    print_step "Waiting for services to be ready..."
    sleep 10
    
    # Wait for PostgreSQL
    local count=0
    until docker compose exec postgres pg_isready -U devuser > /dev/null 2>&1; do
        count=$((count + 1))
        if [ $count -gt 30 ]; then
            print_error "PostgreSQL failed to start"
            exit 1
        fi
        print_step "Waiting for PostgreSQL... ($count/30)"
        sleep 2
    done
    print_success "PostgreSQL is ready"
    
    # Wait for Kafka
    count=0
    until docker compose exec kafka kafka-topics --list --bootstrap-server localhost:9092 > /dev/null 2>&1; do
        count=$((count + 1))
        if [ $count -gt 30 ]; then
            print_error "Kafka failed to start"
            exit 1
        fi
        print_step "Waiting for Kafka... ($count/30)"
        sleep 2
    done
    print_success "Kafka is ready"
}

# Build all projects
build_projects() {
    print_step "Building .NET solution..."
    dotnet restore
    dotnet build --configuration Release
    print_success "✅ .NET projects built successfully"
    
    print_step "Installing frontend dependencies..."
    cd web-frontend
    npm install
    print_success "Frontend dependencies installed"
    cd ..
}

# Run database migrations
run_migrations() {
    print_step "Running database migrations..."
    
    # Install EF tools if not already installed
    if ! dotnet tool list -g | grep -q dotnet-ef; then
        print_step "Installing Entity Framework tools..."
        dotnet tool install --global dotnet-ef
    fi
    
    # Run migrations
    dotnet ef database update --project Employee.Api --startup-project Employee.Api
    print_success "Database migrations completed"
}

# Run tests
run_tests() {
    print_step "Running unit tests..."
    dotnet test --configuration Release --logger trx --results-directory TestResults/
    print_success "All tests passed"
}

# Start all services
start_services() {
    print_step "Starting all services..."
    
    # Create log directory
    mkdir -p logs
    
    # Start API in background
    print_step "Starting Employee API..."
    dotnet run --project Employee.Api > logs/api.log 2>&1 &
    API_PID=$!
    echo $API_PID > logs/api.pid
    
    # Wait a bit for API to start
    sleep 5
    
    # Start Worker in background  
    print_step "Starting Employee Worker..."
    dotnet run --project Employee.Worker > logs/worker.log 2>&1 &
    WORKER_PID=$!
    echo $WORKER_PID > logs/worker.pid
    
    # Start Frontend in background
    print_step "Starting React frontend..."
    cd web-frontend
    npm run dev > ../logs/frontend.log 2>&1 &
    FRONTEND_PID=$!
    echo $FRONTEND_PID > ../logs/frontend.pid
    cd ..
    
    print_success "All services started!"
    echo ""
    echo "🌐 Services available at:"
    echo "   • API + Swagger: http://localhost:5000/swagger"
    echo "   • React App: http://localhost:5173"
    echo "   • PostgreSQL: localhost:5432 (devuser/devpass)"
    echo "   • Kafka: localhost:9092"
    echo ""
    echo "📋 Management commands:"
    echo "   • View logs: $0 logs"
    echo "   • Stop all: $0 stop"
    echo "   • Restart: $0 restart"
}

# Stop all services
stop_services() {
    print_step "Stopping all services..."
    
    # Stop .NET services
    if [ -f logs/api.pid ]; then
        local api_pid=$(cat logs/api.pid)
        if kill -0 $api_pid 2>/dev/null; then
            kill $api_pid
            print_success "API service stopped"
        fi
        rm -f logs/api.pid
    fi
    
    if [ -f logs/worker.pid ]; then
        local worker_pid=$(cat logs/worker.pid)
        if kill -0 $worker_pid 2>/dev/null; then
            kill $worker_pid
            print_success "Worker service stopped"
        fi
        rm -f logs/worker.pid
    fi
    
    if [ -f logs/frontend.pid ]; then
        local frontend_pid=$(cat logs/frontend.pid)
        if kill -0 $frontend_pid 2>/dev/null; then
            kill $frontend_pid
            print_success "Frontend service stopped"
        fi
        rm -f logs/frontend.pid
    fi
    
    # Stop Docker services
    docker compose down
    print_success "Infrastructure services stopped"
}

# Show logs
show_logs() {
    local service=${1:-"all"}
    
    case $service in
        "api")
            tail -f logs/api.log
            ;;
        "worker") 
            tail -f logs/worker.log
            ;;
        "frontend")
            tail -f logs/frontend.log
            ;;
        "kafka")
            docker compose logs -f kafka
            ;;
        "postgres")
            docker compose logs -f postgres
            ;;
        *)
            print_step "Available log streams:"
            echo "  • API logs: $0 logs api"
            echo "  • Worker logs: $0 logs worker"  
            echo "  • Frontend logs: $0 logs frontend"
            echo "  • Kafka logs: $0 logs kafka"
            echo "  • PostgreSQL logs: $0 logs postgres"
            ;;
    esac
}

# Health check
health_check() {
    print_step "Running health checks..."
    
    # Check API
    if curl -s http://localhost:5000/api/claims > /dev/null; then
        print_success "API is responding"
    else
        print_error "API is not responding"
    fi
    
    # Check Frontend
    if curl -s http://localhost:5173 > /dev/null; then
        print_success "Frontend is responding"
    else
        print_error "Frontend is not responding"
    fi
    
    # Check Database
    if docker compose exec postgres pg_isready -U devuser > /dev/null 2>&1; then
        print_success "PostgreSQL is ready"
    else
        print_error "PostgreSQL is not ready"
    fi
    
    # Check Kafka
    if docker compose exec kafka kafka-topics --list --bootstrap-server localhost:9092 > /dev/null 2>&1; then
        print_success "Kafka is ready"
        echo "Available topics:"
        docker compose exec kafka kafka-topics --list --bootstrap-server localhost:9092 | sed 's/^/  • /'
    else
        print_error "Kafka is not ready"
    fi
}

# Show usage information
show_usage() {
    echo "Employee System Management Script"
    echo ""
    echo "USAGE:"
    echo "  $0 <command> [options]"
    echo ""
    echo "COMMANDS:"
    echo "  setup      - Complete project setup (run once)"
    echo "  start      - Start all services"
    echo "  stop       - Stop all services" 
    echo "  restart    - Restart all services"
    echo "  build      - Build all projects"
    echo "  test       - Run tests"
    echo "  logs       - Show logs [api|worker|frontend|kafka|postgres]"
    echo "  health     - Check service health"
    echo "  clean      - Clean build artifacts and stop services"
    echo "  reset      - Reset database and restart services"
    echo ""
    echo "EXAMPLES:"
    echo "  $0 setup           # First time setup"
    echo "  $0 start           # Start everything"
    echo "  $0 logs api        # View API logs"
    echo "  $0 health          # Check if services are running"
}

# Clean everything
clean_system() {
    print_step "Cleaning system..."
    stop_services
    docker compose down -v --remove-orphans
    rm -rf logs/
    dotnet clean
    rm -rf */bin */obj
    cd web-frontend && rm -rf node_modules dist && cd ..
    print_success "System cleaned"
}

# Reset database
reset_database() {
    print_step "Resetting database..."
    stop_services
    docker compose down postgres
    docker volume rm employee-system_pgdata 2>/dev/null || true
    setup_infrastructure
    run_migrations
    print_success "Database reset complete"
}

# Main script logic
main() {
    print_banner
    
    local command=${1:-"help"}
    
    case $command in
        "setup")
            print_step "🚀 Setting up Employee System..."
            check_docker
            check_dotnet  
            check_node
            setup_infrastructure
            build_projects
            run_migrations
            run_tests
            print_success "🎉 Setup complete! Run '$0 start' to launch all services."
            ;;
        "start")
            check_docker
            setup_infrastructure
            start_services
            ;;
        "stop")
            stop_services
            ;;
        "restart")
            stop_services
            sleep 2
            check_docker
            setup_infrastructure  
            start_services
            ;;
        "build")
            build_projects
            ;;
        "test")
            run_tests
            ;;
        "logs")
            show_logs $2
            ;;
        "health")
            health_check