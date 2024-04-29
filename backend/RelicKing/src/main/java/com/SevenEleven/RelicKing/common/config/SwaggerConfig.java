package com.SevenEleven.RelicKing.common.config;

import org.springdoc.core.models.GroupedOpenApi;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import io.swagger.v3.oas.annotations.OpenAPIDefinition;
import io.swagger.v3.oas.annotations.info.Info;
import lombok.RequiredArgsConstructor;

@OpenAPIDefinition(
	info = @Info(title = "RelicKing API 명세서",
		description = "SSAFY 자율 D211 RelicKing API 명세서"))
@RequiredArgsConstructor
@Configuration
public class SwaggerConfig {

	@Bean
	public GroupedOpenApi chatOpenApi() {
		// "/v1/**" 경로에 매칭되는 API를 그룹화하여 문서화한다.
		// String[] paths = {"v1/**"};
		String[] paths = {"/**"};

		return GroupedOpenApi.builder()
			.group("RelicKing API v1")  // 그룹 이름을 설정한다.
			.pathsToMatch(paths)     // 그룹에 속하는 경로 패턴을 지정한다.
			.build();
	}

}
