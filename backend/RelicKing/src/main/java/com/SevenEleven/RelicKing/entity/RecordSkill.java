package com.SevenEleven.RelicKing.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Embeddable;
import lombok.AccessLevel;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Embeddable
@Getter
@Builder
@AllArgsConstructor
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@ToString
public class RecordSkill {

	@Column(nullable = false)
	private int skillNo;

	@Column(nullable = false)
	private int level;

	@Column(nullable = false)
	private int slot;

}
