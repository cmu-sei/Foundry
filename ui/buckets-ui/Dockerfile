#
#multi-stage target: dev
#
FROM node as dev
ARG commit
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN $(npm bin)/ng build --prod --output-path /app/dist
RUN sed -i s/##COMMIT##/"$commit"/ /app/dist/assets/config/settings.json
CMD ["npm", "serve"]

#
#multi-stage target: prod
#
FROM nginx:alpine
WORKDIR /var/www
COPY --from=dev /app/nginx-custom.conf /etc/nginx/conf.d/default.conf
COPY --from=dev /app/nginx-basehref.sh /docker-entrypoint.d/90-basehref.sh
COPY --from=dev /app/dist .
EXPOSE 80
