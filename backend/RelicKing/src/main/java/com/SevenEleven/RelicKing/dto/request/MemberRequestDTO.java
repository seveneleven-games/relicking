package com.SevenEleven.RelicKing.dto.request;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.PositiveOrZero;
import jakarta.validation.constraints.Size;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class MemberRequestDTO {

	private int memberId;

	@Email
	@Size(min = 3, max = 255)
	private String email;

	@Size(max = 12)
	private String nickname;

	private String password;

	@Builder.Default
	@PositiveOrZero
	private int gacha = 0;

	@Builder.Default
	@PositiveOrZero
	private int currentClassNo = 0;
}
