package com.SevenEleven.RelicKing.common.config;

import java.util.Collections;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import io.swagger.v3.oas.annotations.OpenAPIDefinition;
import io.swagger.v3.oas.annotations.info.Info;
import io.swagger.v3.oas.models.Components;
import io.swagger.v3.oas.models.OpenAPI;
import io.swagger.v3.oas.models.security.SecurityRequirement;
import io.swagger.v3.oas.models.security.SecurityScheme;
import io.swagger.v3.oas.models.servers.Server;
import lombok.RequiredArgsConstructor;

@OpenAPIDefinition(info = @Info(
	title = "RelicKing API 명세서",
	description = "SSAFY 자율 D211 RelicKing API 명세서"
))
@RequiredArgsConstructor
@Configuration
public class SwaggerConfig {

	@Value("${server.url}")
	private String serverUrl;
	@Value("${server.description}")
	private String serverDescription;

	@Bean
	public OpenAPI openAPI() {

		SecurityScheme securityScheme = new SecurityScheme()
			.type(SecurityScheme.Type.HTTP).scheme("bearer").bearerFormat("JWT")
			.in(SecurityScheme.In.HEADER).name("Authorization");
		SecurityRequirement securityRequirement = new SecurityRequirement().addList("bearerAuth");

		return new OpenAPI()
			.components(new Components().addSecuritySchemes("bearerAuth", securityScheme))
			.security(Collections.singletonList(securityRequirement))
			.addServersItem(new Server().url(serverUrl).description(serverDescription));
	}
}