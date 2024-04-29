package com.SevenEleven.RelicKing.entity;

import com.SevenEleven.RelicKing.dto.model.MemberRelicDTO;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Entity
@Getter
@Builder
@AllArgsConstructor
@NoArgsConstructor
@ToString
public class MemberRelic {
	@Id
	@Column(name = "member_relic_id")
	@GeneratedValue(strategy = GenerationType.IDENTITY)
	private int memberRelicId;

	@ManyToOne(fetch = FetchType.LAZY)
	@JoinColumn(name = "member_id", referencedColumnName = "member_id", nullable = false)
	private Member member;

	@Builder.Default
	@Column(nullable = false)
	private int relicNo = 0;

	@Builder.Default
	@Column(nullable = false)
	private int level = 1;

	@Builder.Default
	@Column(nullable = false)
	private int exp = 0;

	@Builder.Default
	@Column(nullable = false)
	private int slot = 0;

	public static MemberRelicDTO entityToDTO(MemberRelic memberRelic) {
		return MemberRelicDTO.builder()
			.relicNo(memberRelic.getRelicNo())
			.level(memberRelic.getLevel())
			.exp(memberRelic.getExp())
			.slot(memberRelic.getSlot())
			.build();
	}
}
