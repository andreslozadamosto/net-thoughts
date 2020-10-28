FROM continuumio/anaconda3:latest

# Update system
RUN apt-get update && apt-get -y update

# Add netcore sources
RUN wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb

# Install netcore 3.1
RUN apt-get update; \
  apt-get install -y apt-transport-https && \
  apt-get update && \
  apt-get install -y dotnet-sdk-3.1

# Add dotnet tools to the path
ENV PATH=$PATH:/root/.dotnet/tools

# Install dotnet-try tool
RUN dotnet tool install -g dotnet-try

# Install net provider for Jupyter
RUN dotnet try jupyter install

# Update conda version
RUN conda update conda
RUN conda update conda-build

# update conda plugins
RUN conda update --all

# Jupyter listens port: 8888
EXPOSE 8888

# Run Jupytewr notebook as Docker main process
ENTRYPOINT [ "/opt/conda/bin/jupyter", "notebook", "--allow-root", "--notebook-dir=/opt/notebooks", "--ip='*'", "--port=8888", "--no-browser", "--allow-root" ]