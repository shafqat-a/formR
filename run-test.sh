#!/bin/bash

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
API_PORT=5000
WEB_PORT=3000
DB_HOST=localhost
DB_PORT=5400
DB_NAME=formR
DB_USER=postgres

# Directories
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
API_DIR="$SCRIPT_DIR/src/FormR.API"
WEB_DIR="$SCRIPT_DIR/src/FormR.Web"
SAMPLE_APP_DIR="$SCRIPT_DIR/tests/SampleApp"

# PID files
API_PID_FILE="/tmp/formr-api.pid"
WEB_PID_FILE="/tmp/formr-web.pid"

# Functions
print_header() {
    echo -e "${BLUE}================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}================================${NC}"
}

print_info() {
    echo -e "${GREEN}✓${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}⚠${NC} $1"
}

print_error() {
    echo -e "${RED}✗${NC} $1"
}

check_postgres() {
    print_header "Checking PostgreSQL"

    if ! command -v psql &> /dev/null; then
        print_error "PostgreSQL client (psql) not found"
        exit 1
    fi

    if ! pg_isready -h $DB_HOST -p $DB_PORT &> /dev/null; then
        print_error "PostgreSQL is not running on $DB_HOST:$DB_PORT"
        print_info "Please start PostgreSQL first"
        exit 1
    fi

    print_info "PostgreSQL is running on $DB_HOST:$DB_PORT"

    # Check if database exists
    if psql -h $DB_HOST -p $DB_PORT -U $DB_USER -lqt | cut -d \| -f 1 | grep -qw $DB_NAME; then
        print_info "Database '$DB_NAME' exists"
    else
        print_warning "Database '$DB_NAME' does not exist"
        print_info "It will be created automatically when the API starts"
    fi

    echo ""
}

start_api() {
    print_header "Starting FormR API"

    cd "$API_DIR"

    # Kill existing API process if running
    if [ -f "$API_PID_FILE" ]; then
        OLD_PID=$(cat "$API_PID_FILE")
        if ps -p $OLD_PID > /dev/null 2>&1; then
            print_info "Stopping existing API process (PID: $OLD_PID)"
            kill $OLD_PID 2>/dev/null || true
            sleep 2
        fi
        rm -f "$API_PID_FILE"
    fi

    # Start API in background
    print_info "Starting API on http://localhost:$API_PORT"
    dotnet run --no-build > /tmp/formr-api.log 2>&1 &
    API_PID=$!
    echo $API_PID > "$API_PID_FILE"

    # Wait for API to be ready
    print_info "Waiting for API to be ready..."
    for i in {1..30}; do
        if curl -s http://localhost:$API_PORT/api/v1/controls/library > /dev/null 2>&1; then
            print_info "API is ready!"
            echo ""
            return 0
        fi
        sleep 1
    done

    print_error "API failed to start within 30 seconds"
    print_error "Check logs at /tmp/formr-api.log"
    cat /tmp/formr-api.log
    exit 1
}

start_web() {
    print_header "Starting FormR Web UI"

    cd "$WEB_DIR"

    # Kill existing web process if running
    if [ -f "$WEB_PID_FILE" ]; then
        OLD_PID=$(cat "$WEB_PID_FILE")
        if ps -p $OLD_PID > /dev/null 2>&1; then
            print_info "Stopping existing web process (PID: $OLD_PID)"
            kill $OLD_PID 2>/dev/null || true
            sleep 2
        fi
        rm -f "$WEB_PID_FILE"
    fi

    # Start web in background
    print_info "Starting Web UI on http://localhost:$WEB_PORT"
    npm run dev > /tmp/formr-web.log 2>&1 &
    WEB_PID=$!
    echo $WEB_PID > "$WEB_PID_FILE"

    # Wait for web to be ready
    print_info "Waiting for Web UI to be ready..."
    for i in {1..30}; do
        if curl -s http://localhost:$WEB_PORT > /dev/null 2>&1; then
            print_info "Web UI is ready!"
            echo ""
            return 0
        fi
        sleep 1
    done

    print_error "Web UI failed to start within 30 seconds"
    print_error "Check logs at /tmp/formr-web.log"
    exit 1
}

run_sample_app() {
    print_header "Running Sample Application"

    cd "$SAMPLE_APP_DIR"

    print_info "Building sample app..."
    dotnet build > /dev/null 2>&1

    print_info "Running sample app...\n"
    API_URL="http://localhost:$API_PORT" dotnet run --no-build

    echo ""
}

stop_services() {
    print_header "Stopping Services"

    if [ -f "$API_PID_FILE" ]; then
        API_PID=$(cat "$API_PID_FILE")
        if ps -p $API_PID > /dev/null 2>&1; then
            print_info "Stopping API (PID: $API_PID)"
            kill $API_PID 2>/dev/null || true
        fi
        rm -f "$API_PID_FILE"
    fi

    if [ -f "$WEB_PID_FILE" ]; then
        WEB_PID=$(cat "$WEB_PID_FILE")
        if ps -p $WEB_PID > /dev/null 2>&1; then
            print_info "Stopping Web UI (PID: $WEB_PID)"
            kill $WEB_PID 2>/dev/null || true
        fi
        rm -f "$WEB_PID_FILE"
    fi

    echo ""
}

show_status() {
    print_header "Service Status"

    if [ -f "$API_PID_FILE" ]; then
        API_PID=$(cat "$API_PID_FILE")
        if ps -p $API_PID > /dev/null 2>&1; then
            print_info "API is running (PID: $API_PID) - http://localhost:$API_PORT"
        else
            print_warning "API PID file exists but process is not running"
        fi
    else
        print_warning "API is not running"
    fi

    if [ -f "$WEB_PID_FILE" ]; then
        WEB_PID=$(cat "$WEB_PID_FILE")
        if ps -p $WEB_PID > /dev/null 2>&1; then
            print_info "Web UI is running (PID: $WEB_PID) - http://localhost:$WEB_PORT"
        else
            print_warning "Web UI PID file exists but process is not running"
        fi
    else
        print_warning "Web UI is not running"
    fi

    echo ""
}

show_usage() {
    echo "Usage: $0 [command]"
    echo ""
    echo "Commands:"
    echo "  start       - Start API and Web UI"
    echo "  stop        - Stop API and Web UI"
    echo "  restart     - Restart API and Web UI"
    echo "  test        - Run sample application tests"
    echo "  full        - Start services, run tests, and open browser (default)"
    echo "  status      - Show service status"
    echo "  logs        - Show service logs"
    echo ""
}

show_logs() {
    print_header "Service Logs"

    echo -e "${BLUE}API Logs:${NC}"
    tail -20 /tmp/formr-api.log 2>/dev/null || echo "No API logs found"
    echo ""

    echo -e "${BLUE}Web Logs:${NC}"
    tail -20 /tmp/formr-web.log 2>/dev/null || echo "No Web logs found"
    echo ""
}

# Main script
COMMAND=${1:-full}

case "$COMMAND" in
    start)
        check_postgres
        cd "$API_DIR" && dotnet build
        cd "$WEB_DIR" && npm install
        start_api
        start_web
        show_status
        print_info "FormR is running!"
        print_info "  API:    http://localhost:$API_PORT"
        print_info "  Web UI: http://localhost:$WEB_PORT"
        ;;

    stop)
        stop_services
        print_info "Services stopped"
        ;;

    restart)
        stop_services
        check_postgres
        start_api
        start_web
        show_status
        ;;

    test)
        if ! curl -s http://localhost:$API_PORT/api/v1/controls/library > /dev/null 2>&1; then
            print_error "API is not running. Please start it first with: $0 start"
            exit 1
        fi
        run_sample_app
        ;;

    full)
        check_postgres
        cd "$API_DIR" && dotnet build
        cd "$WEB_DIR" && npm install
        start_api
        start_web
        run_sample_app
        show_status

        print_header "Opening Browser"
        print_info "Opening Web UI at http://localhost:$WEB_PORT"

        # Open browser (cross-platform)
        if command -v xdg-open > /dev/null; then
            xdg-open "http://localhost:$WEB_PORT" &
        elif command -v open > /dev/null; then
            open "http://localhost:$WEB_PORT" &
        else
            print_warning "Could not open browser automatically"
            print_info "Please open http://localhost:$WEB_PORT manually"
        fi

        echo ""
        print_info "Press Ctrl+C to stop all services"

        # Wait for Ctrl+C
        trap stop_services EXIT INT TERM
        wait
        ;;

    status)
        show_status
        ;;

    logs)
        show_logs
        ;;

    *)
        show_usage
        exit 1
        ;;
esac
