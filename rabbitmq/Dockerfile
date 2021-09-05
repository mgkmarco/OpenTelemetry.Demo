ARG PLUGIN_VERSION=1.0.0
# Use the latest stable RabbitMQ Management Decompression plugin
ARG PLUGIN_NAMED_VERSION="rabbitmq_management_decompression-$PLUGIN_VERSION.ez"

# The official Canonical Ubuntu Focal image is ideal from a security perspective
FROM ubuntu:20.04 AS ubuntu
ARG PLUGIN_VERSION
ARG PLUGIN_NAMED_VERSION

# Install dependencies required
RUN set -eux; \
	\
	savedAptMark="$(apt-mark showmanual)"; \
	apt-get update; \
	apt-get install --yes --no-install-recommends \
	ca-certificates \
	wget; \
	\
	RABBITMQ_MANAGEMENT_DECOMPRESSION="https://github.com/mgkmarco/rabbitmq-management-decompression/releases/download/$PLUGIN_VERSION/$PLUGIN_NAMED_VERSION"; \
	\
	wget --progress dot:giga "$RABBITMQ_MANAGEMENT_DECOMPRESSION";

# Pull alpine image. Internally it will pull the management-alpine plugin and expose port 15672
# Checkout https://github.com/docker-library/rabbitmq/blob/7e63843da6bfb191ddee6dbe3dd7ec0df36ae70b/3.8/alpine/management/Dockerfile
FROM rabbitmq:management-alpine AS rabbit
ARG PLUGIN_NAMED_VERSION
ARG PLUGINS_FOLDER=plugins
COPY --from=ubuntu "$PLUGIN_NAMED_VERSION" "/$PLUGINS_FOLDER"
RUN cd "$PLUGINS_FOLDER"; \
	chown rabbitmq:rabbitmq "$PLUGIN_NAMED_VERSION";

# Enable consistent hash plugin for the exchange
RUN rabbitmq-plugins enable rabbitmq_consistent_hash_exchange

# Enable shovel and shovel management plugin 
RUN rabbitmq-plugins enable rabbitmq_shovel
RUN rabbitmq-plugins enable rabbitmq_shovel_management

# Enable consistent hash plugin for the exchange
RUN rabbitmq-plugins enable rabbitmq_management_decompression

# Use --build-arg=definitions=<custom-definitions.json> to override at build time
ARG definitions=definitions.json  
# Copy passhash.sh and if it exists the $definitions json file 
COPY passhash.sh $definitions* /usr/local/bin/

# Copy definitions over to the actual location if it is passed as a variable
# Give ownership to the rabbitmq user and other intermediate steps
RUN echo -e "#!/bin/bash\n" >> /usr/local/bin/def_script.sh
RUN echo -e "if [ -f /usr/local/bin/$definitions ]; then\n" \
            "mv /usr/local/bin/$definitions /etc/rabbitmq;\n" \ 
            "cd /etc/rabbitmq/;\n" \
            "mv $definitions definitions.json;\n" \ 
            "echo loopback_users.guest = false >> /etc/rabbitmq/rabbitmq.conf;\n" \
            "echo listeners.tcp.default = 5672 >> /etc/rabbitmq/rabbitmq.conf;\n" \
            "echo management.tcp.port = 15672 >> /etc/rabbitmq/rabbitmq.conf;\n" \
            "echo "load_definitions = /etc/rabbitmq/definitions.json" >> /etc/rabbitmq/rabbitmq.conf;\n" \
            "chown rabbitmq:rabbitmq rabbitmq.conf;\n" \
         "fi\n" >> /usr/local/bin/def_script.sh;
         
# Swith to directory, set as executable, execute script
RUN cd /usr/local/bin; \
    chmod +x def_script.sh; \
    def_script.sh;