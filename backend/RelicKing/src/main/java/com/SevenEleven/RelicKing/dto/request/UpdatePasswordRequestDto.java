package com.SevenEleven.RelicKing.dto.request;

import com.SevenEleven.RelicKing.common.validation.NotBlankGroup;
import com.SevenEleven.RelicKing.common.validation.PatternGroup;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Pattern;
import lombok.Getter;

@Getter
public class UpdatePasswordRequestDto {

	@NotBlank(message = "비밀번호는 필수 입력 값입니다.", groups = NotBlankGroup.class)
	@Pattern(
		regexp = "^(?=.*[A-Za-z])(?=.*\\d)(?=.*[!@#$%^&*()_+=-])[A-Za-z\\d!@#$%^&*()_+=-]{8,16}$",
		message = "비밀번호는 영문, 숫자, 특수문자를 각각 최소 하나씩 포함하며 8~16자여야 합니다.",
		groups = PatternGroup.class
	)
	private String password;
}
