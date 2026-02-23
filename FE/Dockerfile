# Use Node.js base image
FROM node:20-alpine

# Set working directory
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy source code
COPY . .

# Build the app
RUN npm run build

# Expose port 5173
EXPOSE 5173

# Serve the app
CMD ["npm", "run", "preview", "--", "--port", "5173", "--host"]
