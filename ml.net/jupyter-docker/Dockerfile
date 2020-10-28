FROM jupyter/minimal-notebook

#user
ARG NB_USER=jovyan
ARG NB_UID=1000
ENV USER ${NB_USER}
ENV NB_UID ${NB_UID}
ENV HOME /home/${NB_USER}

WORKDIR ${HOME}

USER root

# Update system
#RUN apt-get update && apt-get -y update

# Add netcore sources
RUN wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN sudo dpkg -i packages-microsoft-prod.deb

# Install netcore 3.1
RUN sudo apt-get update;
RUN sudo apt-get install -y apt-transport-https
RUN sudo apt-get update
RUN sudo apt-get install -y dotnet-sdk-3.1

# Enable detection of running in a container
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    NUGET_XMLDOC_MODE=skip \
    DOTNET_TRY_CLI_TELEMETRY_OPTOUT=true

# run first time
RUN dotnet help

# nuget configuration
COPY ./NuGet.config ${HOME}/nuget.config

# user
RUN chown -R ${NB_UID} ${HOME}
USER ${USER}

# Install dotnet interactive tool
RUN dotnet tool install -g Microsoft.dotnet-interactive --add-source "https://dotnet.myget.org/F/dotnet-try/api/v3/index.json"

# Dotnet tools to path
ENV PATH="${PATH}:${HOME}/.dotnet/tools"
RUN echo "$PATH"

# Install dotnet kernel for Jupyter
RUN dotnet interactive jupyter install

# dotnet telemetry
ENV DOTNET_TRY_CLI_TELEMETRY_OPTOUT=false

# Update conda version
#RUN conda update conda
#RUN conda update conda-build

# update conda plugins
#RUN conda update --all

WORKDIR ${HOME}/work